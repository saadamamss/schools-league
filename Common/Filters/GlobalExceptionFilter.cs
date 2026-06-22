using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var requestId = context.HttpContext.Items["RequestId"]?.ToString() ?? "N/A";

        _logger.LogError(context.Exception, "[{RequestId}] Unhandled exception: {Message}", requestId, context.Exception.Message);

        var response = new ApiResponse<object>
        {
            Data = null,
            Status = new ApiStatus
            {
                Message = "An unexpected error occurred.",
                Code = AppCodes.InternalError,
                Success = false
            }
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = 500,
        };

        context.ExceptionHandled = true;
    }
}
