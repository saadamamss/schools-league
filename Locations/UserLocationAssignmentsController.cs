using Locations;
using Common;
using Common.Models;
using Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LocationsNamespace;

[ApiController]
[Route("api/dashboard/user-location-assignments")]
[SwaggerTag("User Location Assignments")]
[Authorize]
public class UserLocationAssignmentsController : BaseController
{
    private readonly UserLocationAssignmentsService _service;

    public UserLocationAssignmentsController(UserLocationAssignmentsService service) { _service = service; }

    [HttpGet]
    [SwaggerOperation("Get paginated assignments")]
    [ProducesResponseType(typeof(ApiResponse<UserLocationAssignmentDto>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int per_page = 10,
        [FromQuery] int? user_id = null, [FromQuery] int? location_id = null)
    {
        var result = await _service.GetAllAsync(page, per_page, user_id, location_id);
        return OkPaginated(result.Data, result.TotalObjects, result.PerPage, result.CurrentPage);
    }

    [HttpPost]
    [SwaggerOperation("Create assignment")]
    [ProducesResponseType(typeof(ApiResponse<UserLocationAssignmentDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateUserLocationAssignmentDto dto) =>
        (await _service.CreateAsync(dto)).ToActionResult();

    [HttpPut("{id}")]
    [SwaggerOperation("Update assignment")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserLocationAssignmentDto dto) =>
        (await _service.UpdateAsync(id, dto)).ToActionResult();
}
