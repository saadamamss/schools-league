using ClosedXML.Excel;
using Common;
using Common.Models;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Users;

public class UsersService
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public UsersService(AppDbContext db, UserManager<User> userManager) { _db = db; _userManager = userManager; }

    public async Task<PaginatedResult<UserSummaryDto>> GetAllAsync(int page, int perPage, string? search, string? gender, int? userTypeId, int? cityId, DateTime? createdAt = null)
    {
        var query = _db.Users.Include(u => u.City).Include(u => u.UserType).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search) || u.Email!.Contains(search) || (u.SaId != null && u.SaId.Contains(search)));
        if (!string.IsNullOrWhiteSpace(gender)) query = query.Where(u => u.Gender == gender);
        if (userTypeId.HasValue) query = query.Where(u => u.UserTypeId == userTypeId);
        if (cityId.HasValue) query = query.Where(u => u.CityId == cityId);
        if (createdAt.HasValue) query = query.Where(u => u.CreatedAt.Date == createdAt.Value.Date);

        var total = await query.CountAsync();
        var users = await query.OrderByDescending(u => u.CreatedAt).Skip((page - 1) * perPage).Take(perPage).ToListAsync();

        // Batch-load roles for all users on this page (fix N+1)
        var userIds = users.Select(u => u.Id).ToList();
        var userRoles = await _db.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
            .GroupBy(x => x.UserId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Name!).ToList());

        return new PaginatedResult<UserSummaryDto>
        {
            Data = users.Select(u => MapToSummaryDto(u, userRoles.GetValueOrDefault(u.Id, []))).ToList(),
            TotalObjects = total,
            PerPage = perPage,
            CurrentPage = page,
        };
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            return Result<UserDto>.Conflict("Email already in use.");

        // Check if a soft-deleted user exists with the same SaId
        var existingDeleted = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.SaId == dto.SaId && u.IsDeleted);
        if (existingDeleted != null)
            return Result<UserDto>.Conflict("A deleted user with this national ID already exists. Restore instead of re-creating.");

        var user = new User
        {
            UserName = dto.Email, Email = dto.Email, PhoneNumber = dto.Phone,
            FullName = BuildFullName(dto.FirstName, dto.MiddleName, dto.LastName),
            FirstName = dto.FirstName, MiddleName = dto.MiddleName, LastName = dto.LastName,
            SaId = dto.SaId, Gender = dto.Gender, CityId = dto.CityId,
            UserTypeId = dto.UserTypeId, NationalityId = dto.NationalityId,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<UserDto>.ValidationError(string.Join(", ", errors));
        }

        if (dto.Roles.Count != 0)
            await _userManager.AddToRolesAsync(user, dto.Roles);
        else
            await _userManager.AddToRoleAsync(user, "User");

        return Result<UserDto>.Ok(MapToDto(user), "User created successfully.", AppCodes.Created);
    }

    public async Task<Result<UserDto>> UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return Result<UserDto>.NotFound("User not found.");

        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.MiddleName != null) user.MiddleName = dto.MiddleName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.Phone != null) user.PhoneNumber = dto.Phone;
        if (dto.Gender != null) user.Gender = dto.Gender;
        if (dto.CityId.HasValue) user.CityId = dto.CityId;
        if (dto.UserTypeId.HasValue) user.UserTypeId = dto.UserTypeId;
        if (dto.NationalityId.HasValue) user.NationalityId = dto.NationalityId;
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

        // Fix FullName reconstruction — handle null MiddleName properly
        user.FullName = BuildFullName(user.FirstName, user.MiddleName, user.LastName);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<UserDto>.ValidationError(string.Join(", ", errors));
        }

        // Wrap role update in a transaction
        if (dto.Roles != null)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, dto.Roles);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return Result<UserDto>.Fail("Failed to update roles.", AppCodes.InternalError);
            }
        }

        return Result<UserDto>.Ok(MapToDto(user), "User updated successfully.");
    }

    public async Task<Result<object>> DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return Result<object>.NotFound("User not found.");
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Result<object>.Ok(new { }, "User deleted successfully.");
    }

    public async Task<Result<UserDto>> GetByIdAsync(int id)
    {
        var user = await _db.Users
            .Include(u => u.City)
            .Include(u => u.UserType)
            .Include(u => u.UserLocationAssignments).ThenInclude(ula => ula.Location).ThenInclude(l => l.City)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return Result<UserDto>.NotFound("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        var dto = MapToDto(user, roles.ToList());

        // Batch-load all attendance for current month in one query
        var today = TimeHelper.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthAttendances = await _db.Attendances
            .Where(a => a.UserId == id && a.Date >= monthStart && a.Date <= today)
            .ToListAsync();

        // Today's attendance
        var todayAttendance = monthAttendances.FirstOrDefault(a => a.Date == today);
        if (todayAttendance != null)
        {
            dto.TodayAttendanceStatus = todayAttendance.Status;
            dto.TodayCheckIn = todayAttendance.CheckIn?.ToString("HH:mm");
            dto.TodayCheckOut = todayAttendance.CheckOut?.ToString("HH:mm");
        }

        // Working days count (current month)
        dto.WorkingDaysCount = monthAttendances.Count(a => a.Status == "attendance");

        // Monthly working hours
        var monthRecords = monthAttendances
            .Where(a => a.CheckIn != null && a.CheckOut != null)
            .ToList();
        dto.MonthlyWorkingHours = monthRecords.Count != 0
            ? monthRecords.Sum(r => (r.CheckOut!.Value - r.CheckIn!.Value).TotalHours)
            : 0;

        // User's location assignments (all, not just active)
        var assignments = user.UserLocationAssignments
            .Where(ula => !ula.IsDeleted)
            .OrderByDescending(ula => ula.AssignedAt)
            .ToList();

        dto.Locations = [];
        foreach (var assignment in assignments)
        {
            if (assignment.Location == null) continue;

            // Compute total working hours during this assignment period
            var from = assignment.AssignedAt;
            var to = assignment.UnassignedAt ?? DateTime.UtcNow;
            var attendanceRecords = await _db.Attendances
                .Where(a => a.UserId == id
                    && a.LocationId == assignment.LocationId
                    && !a.IsDeleted
                    && a.CheckIn != null
                    && a.CheckOut != null
                    && a.Date >= from.Date
                    && a.Date <= to.Date)
                .Select(a => new { a.CheckIn, a.CheckOut })
                .ToListAsync();
            var hours = attendanceRecords.Sum(a => (a.CheckOut!.Value - a.CheckIn!.Value).TotalHours);

            dto.Locations.Add(new LocationDto
            {
                Id = assignment.Location.Id,
                Name = assignment.Location.Name,
                Type = assignment.Location.Type,
                Image = assignment.Location.Image,
                AssignedAt = assignment.AssignedAt,
                UnassignedAt = assignment.UnassignedAt,
                City = assignment.Location.City != null
                    ? new CityDto { Id = assignment.Location.City.Id, Name = new() { ["ar"] = assignment.Location.City.NameAr, ["en"] = assignment.Location.City.NameEn } }
                    : null,
                TotalWorkingHours = Math.Round(hours, 2),
            });
        }

        return Result<UserDto>.Ok(dto);
    }

    public async Task<Dictionary<string, object>> GetStatisticsAsync()
    {
        var today = TimeHelper.Today;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);

        // Sequential queries (EF Core does not allow concurrent queries on the same DbContext)
        var totalUsers = await _db.Users.LongCountAsync();
        var dailyAttendance = await _db.Attendances.CountAsync(a => a.Date == today && a.Status == "attendance");
        var dailyAbsence = await _db.Attendances.CountAsync(a => a.Date == today && a.Status == "absent");
        var weeklyRecords = await _db.Attendances
            .Where(a => a.Date >= weekStart && a.Date <= today && a.CheckIn != null && a.CheckOut != null)
            .Select(a => new { a.CheckIn, a.CheckOut })
            .ToListAsync();

        var weeklyHours = weeklyRecords.Count != 0
            ? weeklyRecords.Sum(r => (r.CheckOut!.Value - r.CheckIn!.Value).TotalHours)
            : 0;

        return new Dictionary<string, object>
        {
            ["total_users"] = totalUsers,
            ["total_attendance"] = dailyAttendance,
            ["total_absence"] = dailyAbsence,
            ["total_hours"] = weeklyHours.ToString("F2"),
        };
    }

    public async Task<byte[]> ExportToExcelAsync(string? search, string? gender, int? userTypeId, int? cityId)
    {
        var query = _db.Users.Include(u => u.City).Include(u => u.UserType).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search) || u.Email!.Contains(search) || (u.SaId != null && u.SaId.Contains(search)));
        if (!string.IsNullOrWhiteSpace(gender)) query = query.Where(u => u.Gender == gender);
        if (userTypeId.HasValue) query = query.Where(u => u.UserTypeId == userTypeId);
        if (cityId.HasValue) query = query.Where(u => u.CityId == cityId);

        var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Users");
        ws.RightToLeft = true;
        Func<int, int, IXLCell> c = ws.Cell;

        c(1, 1).Value = "الاسم";
        c(1, 2).Value = "البريد الإلكتروني";
        c(1, 3).Value = "رقم الهاتف";
        c(1, 4).Value = "رقم الهوية";
        c(1, 5).Value = "الجنس";
        c(1, 6).Value = "المدينة";
        c(1, 7).Value = "نوع المستخدم";
        c(1, 8).Value = "الحالة";
        c(1, 9).Value = "تاريخ التسجيل";

        for (int i = 1; i <= 9; i++)
        {
            ws.Cell(1, i).Style.Font.Bold = true;
            ws.Cell(1, i).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        int row = 2;
        foreach (var u in users)
        {
            c(row, 1).Value = u.FullName;
            c(row, 2).Value = u.Email ?? "";
            c(row, 3).Value = u.PhoneNumber ?? "";
            c(row, 4).Value = u.SaId ?? "";
            c(row, 5).Value = u.Gender == "male" ? "ذكر" : u.Gender == "female" ? "أنثى" : "";
            c(row, 6).Value = u.City?.NameAr ?? "";
            c(row, 7).Value = u.UserType?.NameAr ?? "";
            c(row, 8).Value = u.IsActive ? "نشط" : "معطل";
            c(row, 9).Value = u.CreatedAt.ToString("yyyy-MM-dd");
            row++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static string BuildFullName(string firstName, string? middleName, string lastName) =>
        string.Join(" ", new[] { firstName, middleName, lastName }.Where(x => !string.IsNullOrWhiteSpace(x)));

    private UserSummaryDto MapToSummaryDto(User user, List<string> roles) => new()
    {
        Id = user.Id, FullName = user.FullName, FirstName = user.FirstName,
        MiddleName = user.MiddleName, LastName = user.LastName, Email = user.Email!,
        Phone = user.PhoneNumber!, SaId = user.SaId, Gender = user.Gender,
        ProfileImage = user.ProfileImage, IsActive = user.IsActive,
        BirthDate = user.BirthDate, CreatedAt = user.CreatedAt,
        DailyRate = user.DailyRate,
        Permissions = roles,
        City = user.City != null ? new CityDto { Id = user.City.Id, Name = new() { ["ar"] = user.City.NameAr, ["en"] = user.City.NameEn } } : null,
        UserType = user.UserType != null ? new UserTypeDto { Id = user.UserType.Id, Code = user.UserType.Code, Name = new() { ["ar"] = user.UserType.NameAr, ["en"] = user.UserType.NameEn }, Key = user.UserType.Key, IsActive = user.UserType.IsActive } : null,
    };

    private UserDto MapToDto(User user) => MapToDto(user, []);

    private UserDto MapToDto(User user, List<string> roles) => new()
    {
        Id = user.Id, FullName = user.FullName, FirstName = user.FirstName,
        MiddleName = user.MiddleName, LastName = user.LastName, Email = user.Email!,
        Phone = user.PhoneNumber!, SaId = user.SaId, Gender = user.Gender,
        ProfileImage = user.ProfileImage, IsActive = user.IsActive,
        BirthDate = user.BirthDate, CreatedAt = user.CreatedAt,
        DailyRate = user.DailyRate,
        Permissions = roles,
        City = user.City != null ? new CityDto { Id = user.City.Id, Name = new() { ["ar"] = user.City.NameAr, ["en"] = user.City.NameEn } } : null,
        UserType = user.UserType != null ? new UserTypeDto { Id = user.UserType.Id, Code = user.UserType.Code, Name = new() { ["ar"] = user.UserType.NameAr, ["en"] = user.UserType.NameEn }, Key = user.UserType.Key, IsActive = user.UserType.IsActive } : null,
        BankInfo = user.BankName != null ? new BankInfoDto
        {
            BankName = user.BankName,
            AccountNumber = user.AccountNumber,
            Iban = user.Iban,
            SwiftCode = user.SwiftCode,
        } : null,
    };
}
