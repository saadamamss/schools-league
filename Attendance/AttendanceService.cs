using ClosedXML.Excel;
using Common;
using Common.Models;
using Data;
using Data.Queries;
using Microsoft.EntityFrameworkCore;
using static Common.TimeHelper;

namespace Attendance;

public class AttendanceService
{
    private readonly AppDbContext _db;
    private readonly AttendanceStatisticsQuery _statsQueries;
    private readonly UserAttendanceQueries _userStatsQueries;

    public AttendanceService(AppDbContext db, AttendanceStatisticsQuery statsQueries, UserAttendanceQueries userStatsQueries)
    { _db = db; _statsQueries = statsQueries; _userStatsQueries = userStatsQueries; }

    public async Task<PaginatedResult<AttendanceRecordDto>> GetAllAsync(int page, int perPage, string? search, DateTime? date, string? status, int? locationId, int? userId = null)
    {
        var query = _db.Attendances
            .Include(a => a.User).ThenInclude(u => u.City)
            .Include(a => a.User).ThenInclude(u => u.UserType)
            .Include(a => a.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.User.FullName.Contains(search) || a.User.Email!.Contains(search));
        if (date.HasValue) query = query.Where(a => a.Date == date.Value.Date);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(a => a.Status == status);
        if (locationId.HasValue) query = query.Where(a => a.LocationId == locationId);
        if (userId.HasValue) query = query.Where(a => a.UserId == userId);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.Date).Skip((page - 1) * perPage).Take(perPage).ToListAsync();

        return new PaginatedResult<AttendanceRecordDto>
        {
            Data = items.Select(MapToDto).ToList(),
            TotalObjects = total, PerPage = perPage, CurrentPage = page,
        };
    }

    public async Task<Dictionary<string, object>> GetStatisticsAsync() =>
        await _statsQueries.GetDashboardStatsAsync();

    public async Task<byte[]> ExportToExcelAsync(string? search, DateTime? date, string? status, int? locationId, int? userId = null)
    {
        var query = _db.Attendances
            .Include(a => a.User).ThenInclude(u => u.City)
            .Include(a => a.User).ThenInclude(u => u.UserType)
            .Include(a => a.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.User.FullName.Contains(search) || a.User.Email!.Contains(search));
        if (date.HasValue) query = query.Where(a => a.Date == date.Value.Date);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(a => a.Status == status);
        if (locationId.HasValue) query = query.Where(a => a.LocationId == locationId);
        if (userId.HasValue) query = query.Where(a => a.UserId == userId);

        var items = await query.OrderByDescending(a => a.Date).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Attendance");

        ws.RightToLeft = true;
        Func<int, int, IXLCell> c = ws.Cell;

        c(1, 1).Value = "الاسم";
        c(1, 2).Value = "رقم الهوية";
        c(1, 3).Value = "المدينة";
        c(1, 4).Value = "الحالة";
        c(1, 5).Value = "وقت الدخول";
        c(1, 6).Value = "وقت الخروج";
        c(1, 7).Value = "إجمالي الساعات";
        c(1, 8).Value = "الموقع";
        c(1, 9).Value = "التاريخ";

        for (int i = 1; i <= 9; i++)
        {
            ws.Cell(1, i).Style.Font.Bold = true;
            ws.Cell(1, i).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        int row = 2;
        foreach (var a in items)
        {
            var statusMap = a.Status switch
            {
                "attendance" => "حاضر",
                "absent" => "غائب",
                "departed" => "غادر",
                _ => a.Status
            };

            c(row, 1).Value = a.User.FullName;
            c(row, 2).Value = a.User.SaId ?? "";
            c(row, 3).Value = a.User.City?.NameAr ?? "";
            c(row, 4).Value = statusMap;
            c(row, 5).Value = a.CheckIn?.ToString("HH:mm:ss") ?? "";
            c(row, 6).Value = a.CheckOut?.ToString("HH:mm:ss") ?? "";
            c(row, 7).Value = a.CheckIn != null && a.CheckOut != null ? Math.Round((a.CheckOut.Value - a.CheckIn.Value).TotalHours, 2).ToString() : "";
            c(row, 8).Value = a.Location?.Name ?? "";
            c(row, 9).Value = a.Date.ToString("yyyy-MM-dd");
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static AttendanceRecordDto MapToDto(Data.Attendance a) => new()
    {
        Id = a.Id, UserId = a.UserId, LocationId = a.LocationId,
        Status = a.Status, CheckIn = a.CheckIn?.ToString("HH:mm:ss"), CheckOut = a.CheckOut?.ToString("HH:mm:ss"),
        TotalHours = a.CheckIn != null && a.CheckOut != null ? (a.CheckOut.Value - a.CheckIn.Value).TotalHours : null,
        Date = a.Date.ToString("yyyy-MM-dd"),
        User = new UserSummaryDto
        {
            Id = a.User.Id, FullName = a.User.FullName, FirstName = a.User.FirstName,
            LastName = a.User.LastName, Email = a.User.Email!, Phone = a.User.PhoneNumber!,
            SaId = a.User.SaId, Gender = a.User.Gender, ProfileImage = a.User.ProfileImage,
            IsActive = a.User.IsActive,
            City = a.User.City != null ? new CityDto { Id = a.User.City.Id, Name = new() { ["ar"] = a.User.City.NameAr, ["en"] = a.User.City.NameEn } } : null,
            UserType = a.User.UserType != null ? new UserTypeDto { Id = a.User.UserType.Id, Code = a.User.UserType.Code, Name = new() { ["ar"] = a.User.UserType.NameAr, ["en"] = a.User.UserType.NameEn }, Key = a.User.UserType.Key, IsActive = a.User.UserType.IsActive } : null,
        },
        Location = new LocationDto { Id = a.Location.Id, Name = a.Location.Name, Type = a.Location.Type, Image = a.Location.Image },
    };
}
