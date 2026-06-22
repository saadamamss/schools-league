using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Common;

[ApiController]
[EnableRateLimiting("global")]
public abstract class BaseController : ControllerBase
{
    protected int CurrentUserId =>
        int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id)
            ? id : 0;

    protected IActionResult OkResult<T>(T data, string message = "Success") =>
        Ok(new ApiResponse<T> { Data = data, Status = new ApiStatus { Message = message, Code = 200, Success = true } });

    protected IActionResult CreatedResult<T>(T data, string message = "Created") =>
        StatusCode(201, new ApiResponse<T> { Data = data, Status = new ApiStatus { Message = message, Code = 201, Success = true } });

    protected IActionResult OkPaginated<T>(List<T> data, int total, int perPage, int currentPage, Dictionary<string, object>? statistics = null) =>
        Ok(new ApiResponse<List<T>>
        {
            Data = data,
            Pagination = new PaginationMeta { IPerPage = perPage, ITotalObjects = total, ICurrentPage = currentPage },
            Statistics = statistics,
            Status = new ApiStatus { Message = "Success", Code = 200, Success = true },
        });

    protected IActionResult NotFoundResult(string message = "Resource not found.") =>
        NotFound(new ApiResponse<object> { Status = new ApiStatus { Message = message, Code = 404, Success = false } });

    protected IActionResult ErrorResult(string message, int code = 400) =>
        StatusCode(code >= 1000 ? 400 : code, new ApiResponse<object> { Status = new ApiStatus { Message = message, Code = code, Success = false } });
}
