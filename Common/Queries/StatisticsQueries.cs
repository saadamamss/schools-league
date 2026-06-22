using Common;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries;

public class AttendanceStatisticsQuery(AppDbContext db)
{
    public async Task<Dictionary<string, object>> GetDashboardStatsAsync()
    {
        var today = TimeHelper.Today;

        // Removed IgnoreQueryFilters() — soft-deleted records should not be counted
        var totalUsers = await db.Users.CountAsync();
        var totalLocations = await db.Locations.CountAsync();
        var todayAttendance = await db.Attendances
            .CountAsync(a => a.Date == today && a.Status == "attendance");
        var todayAbsence = await db.Attendances
            .CountAsync(a => a.Date == today && a.Status == "absent");

        return new Dictionary<string, object>
        {
            ["total_users"] = totalUsers,
            ["total_locations"] = totalLocations,
            ["total_attendance"] = todayAttendance,
            ["total_absence"] = todayAbsence,
        };
    }
}

public class UserAttendanceQueries(AppDbContext db)
{
    public async Task<Dictionary<string, object>> GetUserStatsAsync()
    {
        var total = await db.Users.CountAsync();
        var active = await db.Users.CountAsync(u => u.IsActive);

        return new Dictionary<string, object>
        {
            ["total_users"] = total,
            ["active_users"] = active,
        };
    }
}

public class LocationStatisticsQueries(AppDbContext db)
{
    public async Task<Dictionary<string, object>> GetLocationStatsAsync()
    {
        var totalLocations = await db.Locations.CountAsync();
        var activeLocations = await db.Locations.CountAsync(l => l.IsActive);

        return new Dictionary<string, object>
        {
            ["total_locations"] = totalLocations,
            ["active_locations"] = activeLocations,
        };
    }
}
