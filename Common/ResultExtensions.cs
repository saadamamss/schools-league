using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? new OkObjectResult(new ApiResponse<T>
            {
                Data = result.Data,
                Status = new ApiStatus { Message = result.Message, Code = result.Code, Success = true },
            })
            : new ObjectResult(new ApiResponse<object>
            {
                Status = new ApiStatus { Message = result.Message, Code = result.Code, Success = false },
            })
            { StatusCode = result.Code >= 1000 ? 400 : result.Code };
}
