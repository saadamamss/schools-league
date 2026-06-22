using Common;
using Common.Models;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace StatisticsNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Statistics")]
[Authorize]
public class StatisticsController : BaseController
{
    private readonly AppDbContext _db;

    public StatisticsController(AppDbContext db) { _db = db; }

    [HttpGet]
    [SwaggerOperation("Get dashboard statistics")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Dashboard(
        [FromQuery] string? attendance_period = "month",
        [FromQuery] string? attendance_by_location_period = "week",
        [FromQuery] string? absence_by_location_period = "week"
    )
    {
        try
        {
            var today = TimeHelper.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            var totalUsers = await _db.Users.LongCountAsync();
            var totalLocations = await _db.Locations.LongCountAsync();
            var dailyAttendance = await _db.Attendances.CountAsync(a => a.Date == today && a.Status == "attendance");
            var weeklyRecords = await _db.Attendances
                .Where(a => a.Date >= weekStart && a.Date <= today && a.CheckIn != null && a.CheckOut != null)
                .Select(a => new { a.CheckIn, a.CheckOut })
                .ToListAsync();
            var weeklyHours = weeklyRecords.Count != 0
                ? weeklyRecords.Sum(r => (r.CheckOut!.Value - r.CheckIn!.Value).TotalHours)
                : 0;

            var attendanceStatsDateStart = attendance_period switch
            {
                "day" => today,
                "week" => weekStart,
                _ => today.AddMonths(-1),
            };
            var attendanceStats = await _db.Attendances
                .Where(a => a.Date >= attendanceStatsDateStart)
                .GroupBy(a => a.Date)
                .Select(g => new
                {
                    date = g.Key,
                    attendance = g.Count(a => a.Status == "attendance"),
                    absence = g.Count(a => a.Status == "absent"),
                })
                .OrderBy(x => x.date)
                .ToListAsync();

            var attendanceByLocationDateStart = attendance_by_location_period switch
            {
                "day" => today,
                "month" => today.AddMonths(-1),
                _ => weekStart,
            };
            var absenceByLocationDateStart = absence_by_location_period switch
            {
                "day" => today,
                "month" => today.AddMonths(-1),
                _ => weekStart,
            };

            var attendanceByLocation = await _db.Attendances
                .Where(a => a.Date >= attendanceByLocationDateStart && a.Location != null)
                .GroupBy(a => a.Location!.Name)
                .Select(g => new
                {
                    location_name = g.Key,
                    attendance_count = g.Count(a => a.Status == "attendance"),
                })
                .OrderByDescending(x => x.attendance_count)
                .ToListAsync();

            var absenceByLocation = await _db.Attendances
                .Where(a => a.Date >= absenceByLocationDateStart && a.Location != null)
                .GroupBy(a => a.Location!.Name)
                .Select(g => new
                {
                    location_name = g.Key,
                    absence_count = g.Count(a => a.Status == "absent"),
                })
                .OrderByDescending(x => x.absence_count)
                .ToListAsync();

            return Ok(new ApiResponse<object>
            {
                Data = new
                {
                    total_users = totalUsers,
                    daily_attendance = dailyAttendance,
                    active_locations = totalLocations,
                    weekly_hours = weeklyHours.ToString("F2"),
                },
                Status = new ApiStatus { Message = "Statistics retrieved successfully.", Code = 200, Success = true },
                Statistics = new Dictionary<string, object>
                {
                    ["attendance_statistics"] = attendanceStats,
                    ["attendance_by_location"] = attendanceByLocation,
                    ["absence_by_location"] = absenceByLocation,
                },
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve statistics");
            return StatusCode(500, new ApiResponse<object>
            {
                Status = new ApiStatus { Message = "Failed to retrieve statistics", Code = 500, Success = false },
            });
        }
    }
}
