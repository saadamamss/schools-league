using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common;
using Common.Models;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Users;
using static Common.TimeHelper;

namespace UsersNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Users")]
[Authorize]
public class UsersController : BaseController
{
    private readonly UsersService _users;

    public UsersController(UsersService users) { _users = users; }

    [HttpGet]
    [SwaggerOperation("Get paginated list of users")]
    [ProducesResponseType(typeof(ApiResponse<UserSummaryDto>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int per_page = 10,
        [FromQuery] string? search = null, [FromQuery] string? gender = null,
        [FromQuery] int? user_type_id = null, [FromQuery] int? city_id = null)
    {
        var result = await _users.GetAllAsync(page, per_page, search, gender, user_type_id, city_id);
        var statistics = await _users.GetStatisticsAsync();
        return OkPaginated(result.Data, result.TotalObjects, result.PerPage, result.CurrentPage, statistics);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation("Get user by ID")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _users.GetByIdAsync(id);
        return result.ToActionResult();
    }

    [HttpPost]
    [SwaggerOperation("Create a new user")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto) =>
        (await _users.CreateAsync(dto)).ToActionResult();

    [HttpPut("{id}")]
    [SwaggerOperation("Update a user")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto) =>
        (await _users.UpdateAsync(id, dto)).ToActionResult();

    [HttpDelete("{id}")]
    [SwaggerOperation("Soft-delete a user")]
    public async Task<IActionResult> Delete(int id) =>
        (await _users.DeleteAsync(id)).ToActionResult();

    [HttpPost("users-export-excel")]
    [SwaggerOperation("Export users to Excel")]
    public async Task<IActionResult> ExportExcel([FromBody] ExportUsersRequest request)
    {
        var bytes = await _users.ExportToExcelAsync(request.Search, request.Gender, request.UserTypeId, request.CityId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"users-{Today:yyyy-MM-dd}.xlsx");
    }
}

public class ExportUsersRequest
{
    public string? Search { get; set; }
    public string? Gender { get; set; }
    [JsonPropertyName("user_type_id")]
    public int? UserTypeId { get; set; }
    [JsonPropertyName("city_id")]
    public int? CityId { get; set; }
}
