using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common;
using Common.Models;
using Data;
using Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static Common.TimeHelper;

namespace LocationsNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Locations")]
[Authorize]
public class LocationsController : BaseController
{
    private readonly LocationsService _locations;

    public LocationsController(LocationsService locations) { _locations = locations; }

    [HttpGet]
    [SwaggerOperation("Get paginated list of locations")]
    [ProducesResponseType(typeof(ApiResponse<LocationListItemDto>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int per_page = 10,
        [FromQuery] string? search = null, [FromQuery] int? city_id = null)
    {
        var result = await _locations.GetAllAsync(page, per_page, search, city_id);
        var stats = await _locations.GetStatisticsAsync();
        return OkPaginated(result.Data, result.TotalObjects, result.PerPage, result.CurrentPage, stats);
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Get location by ID")]
    [ProducesResponseType(typeof(ApiResponse<LocationDetailDto>), 200)]
    public async Task<IActionResult> GetById(int id)
    {
        var details = await _locations.GetDetailsAsync(id);
        if (details == null)
            return NotFound(new { data = (object?)null, status = new { message = "Location not found.", code = 404, success = false } });
        return Ok(new
        {
            data = details,
            status = new { message = "Success", code = 200, success = true },
        });
    }

    [HttpPost]
    [SwaggerOperation("Create a new location")]
    [ProducesResponseType(typeof(ApiResponse<Location>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateLocationDto dto) =>
        (await _locations.CreateAsync(dto)).ToActionResult();

    [HttpPut("{id}")]
    [SwaggerOperation("Update a location")]
    [ProducesResponseType(typeof(ApiResponse<Location>), 200)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLocationDto dto) =>
        (await _locations.UpdateAsync(id, dto)).ToActionResult();

    [HttpDelete("{id}")]
    [SwaggerOperation("Soft-delete a location")]
    public async Task<IActionResult> Delete(int id) =>
        (await _locations.DeleteAsync(id)).ToActionResult();

    [HttpPost("locations-export-excel")]
    [SwaggerOperation("Export locations to Excel")]
    public async Task<IActionResult> ExportExcel([FromBody] ExportLocationsRequest request)
    {
        var bytes = await _locations.ExportToExcelAsync(request.Search, request.CityId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"locations-{Today:yyyy-MM-dd}.xlsx");
    }
}

public class ExportLocationsRequest
{
    public string? Search { get; set; }
    [JsonPropertyName("city_id")]
    public int? CityId { get; set; }
}
