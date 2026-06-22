using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Attendance;
using Common;
using Common.Models;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static Common.TimeHelper;

namespace AttendanceNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Attendance")]
[Authorize]
public class AttendanceController : BaseController
{
    private readonly AttendanceService _attendance;

    public AttendanceController(AttendanceService attendance) { _attendance = attendance; }

    [HttpGet]
    [SwaggerOperation("Get paginated attendance records")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceRecordDto>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int per_page = 10,
        [FromQuery] string? search = null, [FromQuery] DateTime? date = null,
        [FromQuery] string? status = null, [FromQuery] int? location_id = null,
        [FromQuery] int? user_id = null)
    {
        var result = await _attendance.GetAllAsync(page, per_page, search, date, status, location_id, user_id);
        var stats = await _attendance.GetStatisticsAsync();
        return OkPaginated(result.Data, result.TotalObjects, result.PerPage, result.CurrentPage, stats);
    }

    [HttpPost("attendance-export-excel")]
    [SwaggerOperation("Export attendance records to Excel")]
    public async Task<IActionResult> ExportExcel(
        [FromBody] ExportAttendanceRequest request)
    {
        var bytes = await _attendance.ExportToExcelAsync(
            request.Search, request.Date, request.Status, request.LocationId, request.UserId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"attendance-{Today:yyyy-MM-dd}.xlsx");
    }
}

public class ExportAttendanceRequest
{
    public string? Search { get; set; }
    public DateTime? Date { get; set; }
    public string? Status { get; set; }
    [JsonPropertyName("user_id")]
    public int? UserId { get; set; }
    [JsonPropertyName("location_id")]
    public int? LocationId { get; set; }
}
