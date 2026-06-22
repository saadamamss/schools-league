using ClosedXML.Excel;
using Common;
using Common.Models;
using Data;
using Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Locations;

public class LocationsService
{
    private readonly AppDbContext _db;
    private readonly LocationStatisticsQueries _statsQueries;

    public LocationsService(AppDbContext db, LocationStatisticsQueries statsQueries)
    { _db = db; _statsQueries = statsQueries; }

    public async Task<PaginatedResult<LocationListItemDto>> GetAllAsync(int page, int perPage, string? search, int? cityId)
    {
        var query = _db.Locations.Include(l => l.City).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(l => l.Name.Contains(search) || (l.License != null && l.License.Contains(search)));
        if (cityId.HasValue) query = query.Where(l => l.CityId == cityId);

        var total = await query.CountAsync();

        var locations = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        var locationIds = locations.Select(l => l.Id).ToList();

        // Batch load assignment counts (one query instead of N)
        var assignmentCounts = await _db.UserLocationAssignments
            .Where(ula => locationIds.Contains(ula.LocationId) && !ula.IsDeleted && ula.UnassignedAt == null)
            .GroupBy(ula => ula.LocationId)
            .Select(g => new { LocationId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.LocationId, x => x.Count);

        var mapped = locations.Select(l => new LocationListItemDto
        {
            Id = l.Id,
            Name = l.Name ?? "",
            Type = l.Type ?? "",
            License = l.License ?? "",
            Phone = l.Phone ?? "",
            Address = l.Address ?? "",
            JoinedEmployee = l.JoinedEmployee ?? 0,
            ObserverCount = l.ObserverCount ?? 0,
            EmployeeNumber = (l.JoinedEmployee ?? 0) - (l.ObserverCount ?? 0),
            ObserverNumber = l.ObserverCount ?? 0,
            WorkHours = l.WorkHours ?? "0",
            Image = l.Image ?? "",
            IsActive = l.IsActive,
            CurrentEventsCount = assignmentCounts.GetValueOrDefault(l.Id, 0),
            City = l.City != null
                ? new CityDto
                {
                    Id = l.City.Id,
                    Name = new() { ["ar"] = l.City.NameAr ?? "", ["en"] = l.City.NameEn ?? "" },
                }
                : new CityDto
                {
                    Name = new() { ["ar"] = "", ["en"] = "" },
                },
        }).ToList();

        return new PaginatedResult<LocationListItemDto>
        {
            Data = mapped,
            TotalObjects = total,
            PerPage = perPage,
            CurrentPage = page,
        };
    }

    public async Task<Result<Location>> GetByIdAsync(int id)
    {
        var location = await _db.Locations.Include(l => l.City).FirstOrDefaultAsync(l => l.Id == id);
        if (location == null) return Result<Location>.NotFound("Location not found.");
        return Result<Location>.Ok(location);
    }

    public async Task<LocationDetailDto?> GetDetailsAsync(int id)
    {
        var location = await _db.Locations.Include(l => l.City).FirstOrDefaultAsync(l => l.Id == id);
        if (location == null) return null;

        var today = TimeHelper.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var weekStart = today.AddDays(-(int)today.DayOfWeek);

        // Load all active assignments for this location in one query
        var assignments = await _db.UserLocationAssignments
            .Include(ula => ula.User).ThenInclude(u => u.City)
            .Include(ula => ula.User).ThenInclude(u => u.UserType)
            .Where(ula => ula.LocationId == id && !ula.IsDeleted && ula.UnassignedAt == null)
            .ToListAsync();

        var assignedUserIds = assignments.Select(a => a.UserId).ToList();

        // ── Batch load attendance data (fixes N+1) ──────────────
        // Today's attendance for all assigned users in one query
        var todayAttendances = await _db.Attendances
            .Where(a => assignedUserIds.Contains(a.UserId) && a.Date == today)
            .ToDictionaryAsync(a => a.UserId);

        // Monthly attendance records for all assigned users in one query
        var monthlyRecords = await _db.Attendances
            .Where(a => assignedUserIds.Contains(a.UserId)
                     && a.Date >= monthStart && a.Date <= today
                     && a.CheckIn != null && a.CheckOut != null)
            .Select(a => new { a.UserId, a.CheckIn, a.CheckOut })
            .ToListAsync();

        var monthlyHoursByUser = monthlyRecords
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => Math.Round(g.Sum(r => (r.CheckOut!.Value - r.CheckIn!.Value).TotalHours), 2));

        // Weekly attendance records for the location (overall)
        var weeklyRecords = await _db.Attendances
            .Where(a => a.LocationId == id && a.Date >= weekStart && a.Date <= today
                     && a.CheckIn != null && a.CheckOut != null)
            .Select(a => new { a.CheckIn, a.CheckOut })
            .ToListAsync();
        var weeklyHours = weeklyRecords.Count != 0
            ? Math.Round(weeklyRecords.Sum(r => (r.CheckOut!.Value - r.CheckIn!.Value).TotalHours), 2)
            : 0;

        // ── Build DTOs from batched data ────────────────────────
        EmployeeBriefDto? BuildEmployeeBrief(User user)
        {
            var todayAtt = todayAttendances.GetValueOrDefault(user.Id);
            return new EmployeeBriefDto
            {
                FullName = user.FullName ?? "",
                UserType = user.UserType != null
                    ? new UserTypeDto
                    {
                        Id = user.UserType.Id,
                        Code = user.UserType.Code,
                        Name = new() { ["ar"] = user.UserType.NameAr ?? "", ["en"] = user.UserType.NameEn ?? "" },
                        Key = user.UserType.Key,
                        IsActive = user.UserType.IsActive,
                    }
                    : null,
                City = user.City != null
                    ? new CityDto
                    {
                        Id = user.City.Id,
                        Name = new() { ["ar"] = user.City.NameAr ?? "", ["en"] = user.City.NameEn ?? "" },
                    }
                    : null,
                MonthlyWorkingHours = monthlyHoursByUser.GetValueOrDefault(user.Id, 0),
                TodayCheckIn = todayAtt?.CheckIn?.ToString("HH:mm"),
                TodayCheckOut = todayAtt?.CheckOut?.ToString("HH:mm"),
            };
        }

        var supervisorAssignment = assignments.FirstOrDefault(a =>
            a.Role == "supervisor");
        var employeeAssignments = assignments.Where(a =>
            a.Role != "supervisor").ToList();

        return new LocationDetailDto
        {
            Name = location.Name ?? "",
            Type = location.Type ?? "",
            LicenseNo = location.License ?? "",
            Phone = location.Phone ?? "",
            Address = location.Address ?? "",
            EmployeeNumber = (location.JoinedEmployee ?? 0) - (location.ObserverCount ?? 0),
            ObserverNumber = location.ObserverCount ?? 0,
            ObserverCount = location.ObserverCount ?? 0,
            WorkHours = location.WorkHours ?? "0",
            TotalWorkingHours = weeklyHours,
            MaxUsers = location.JoinedEmployee ?? 0,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Image = location.Image ?? "",
            Supervisor = supervisorAssignment != null
                ? BuildEmployeeBrief(supervisorAssignment.User)
                : null,
            Inspectors = employeeAssignments
                .Select(a => BuildEmployeeBrief(a.User)!)
                .Where(b => b != null)
                .ToList(),
        };
    }

    public async Task<Result<Location>> CreateAsync(CreateLocationDto dto)
    {
        var location = new Location
        {
            Name = dto.Name,
            Type = dto.Type,
            License = dto.License,
            Phone = dto.Phone,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            CityId = dto.CityId,
            Image = dto.Image,
            JoinedEmployee = dto.JoinedEmployee,
            WorkHours = dto.WorkHours,
            WorkType = dto.WorkType,
        };
        _db.Locations.Add(location);
        await _db.SaveChangesAsync();
        return Result<Location>.Ok(location, "Location created successfully.", AppCodes.Created);
    }

    public async Task<Result<Location>> UpdateAsync(int id, UpdateLocationDto dto)
    {
        var location = await _db.Locations.FindAsync(id);
        if (location == null) return Result<Location>.NotFound("Location not found.");

        if (dto.Name != null) location.Name = dto.Name;
        if (dto.Type != null) location.Type = dto.Type;
        if (dto.License != null) location.License = dto.License;
        if (dto.Phone != null) location.Phone = dto.Phone;
        if (dto.Address != null) location.Address = dto.Address;
        if (dto.Latitude.HasValue) location.Latitude = dto.Latitude;
        if (dto.Longitude.HasValue) location.Longitude = dto.Longitude;
        if (dto.CityId.HasValue) location.CityId = dto.CityId;
        if (dto.Image != null) location.Image = dto.Image;
        if (dto.JoinedEmployee.HasValue) location.JoinedEmployee = dto.JoinedEmployee;
        if (dto.WorkHours != null) location.WorkHours = dto.WorkHours;
        if (dto.WorkType != null) location.WorkType = dto.WorkType;
        if (dto.IsActive.HasValue) location.IsActive = dto.IsActive.Value;

        await _db.SaveChangesAsync();
        return Result<Location>.Ok(location, "Location updated successfully.");
    }

    public async Task<Result<object>> DeleteAsync(int id)
    {
        var location = await _db.Locations.FindAsync(id);
        if (location == null) return Result<object>.NotFound("Location not found.");
        location.IsDeleted = true;
        location.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Result<object>.Ok(new { }, "Location deleted successfully.");
    }

    public async Task<byte[]> ExportToExcelAsync(string? search, int? cityId)
    {
        var query = _db.Locations.Include(l => l.City).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(l => l.Name.Contains(search) || (l.License != null && l.License.Contains(search)));
        if (cityId.HasValue) query = query.Where(l => l.CityId == cityId);

        var locations = await query.OrderByDescending(l => l.CreatedAt).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Locations");
        ws.RightToLeft = true;
        Func<int, int, IXLCell> c = ws.Cell;

        c(1, 1).Value = "اسم الموقع";
        c(1, 2).Value = "نوع الموقع";
        c(1, 3).Value = "رقم التصريح";
        c(1, 4).Value = "رقم الهاتف";
        c(1, 5).Value = "العنوان";
        c(1, 6).Value = "المدينة";
        c(1, 7).Value = "الحالة";
        c(1, 8).Value = "تاريخ الإضافة";

        for (int i = 1; i <= 8; i++)
        {
            ws.Cell(1, i).Style.Font.Bold = true;
            ws.Cell(1, i).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        int row = 2;
        foreach (var l in locations)
        {
            c(row, 1).Value = l.Name;
            c(row, 2).Value = l.Type ?? "";
            c(row, 3).Value = l.License ?? "";
            c(row, 4).Value = l.Phone ?? "";
            c(row, 5).Value = l.Address ?? "";
            c(row, 6).Value = l.City?.NameAr ?? "";
            c(row, 7).Value = l.IsActive ? "نشط" : "معطل";
            c(row, 8).Value = l.CreatedAt.ToString("yyyy-MM-dd");
            row++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<Dictionary<string, object>> GetStatisticsAsync() =>
        await _statsQueries.GetLocationStatsAsync();
}
