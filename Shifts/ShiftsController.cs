using Shifts;
using Common;
using Common.Models;
using Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ShiftsNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Shifts")]
[Authorize]
public class ShiftsController : BaseController
{
    private readonly ShiftsService _shifts;

    public ShiftsController(ShiftsService shifts) { _shifts = shifts; }

    [HttpGet]
    [SwaggerOperation("Get paginated shifts")]
    [ProducesResponseType(typeof(ApiResponse<ShiftDto>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int per_page = 10,
        [FromQuery] string? search = null, [FromQuery] string? status = null)
    {
        var result = await _shifts.GetAllAsync(page, per_page, search, status);
        return OkPaginated(result.Data, result.TotalObjects, result.PerPage, result.CurrentPage);
    }

    [HttpPost]
    [SwaggerOperation("Create a shift")]
    [ProducesResponseType(typeof(ApiResponse<ShiftDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateShiftDto dto) =>
        (await _shifts.CreateAsync(dto)).ToActionResult();

    [HttpPut("{id}")]
    [SwaggerOperation("Update a shift")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShiftDto dto) =>
        (await _shifts.UpdateAsync(id, dto)).ToActionResult();
}
