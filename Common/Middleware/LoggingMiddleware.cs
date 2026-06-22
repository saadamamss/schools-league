using System.Text;

namespace Common.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8].ToUpper();
        context.Items["RequestId"] = requestId;
        context.Response.Headers["X-Request-Id"] = requestId;

        var method = context.Request.Method;
        var path = context.Request.Path;
        var start = DateTime.UtcNow;

        // Enable buffering so we can read the request body after forwarding
        context.Request.EnableBuffering();

        await _next(context);

        var ms = (DateTime.UtcNow - start).TotalMilliseconds;
        var status = context.Response.StatusCode;

        var level = status >= 500 ? LogLevel.Error
                  : status >= 400 ? LogLevel.Warning
                  : LogLevel.Information;

        if (status >= 400)
        {
            // Log request body for failed requests to aid debugging
            string? body = null;
            try
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }
            catch
            {
                body = "(unreadable)";
            }

            if (!string.IsNullOrEmpty(body) && body.Length <= 4096)
            {
                _logger.Log(level, "[{RequestId}] {Method} {Path} {Status} — {Ms}ms\nBody: {Body}",
                    requestId, method, path, status, (int)ms, body);
            }
            else
            {
                _logger.Log(level, "[{RequestId}] {Method} {Path} {Status} — {Ms}ms",
                    requestId, method, path, status, (int)ms);
            }
        }
        else
        {
            _logger.Log(level, "[{RequestId}] {Method} {Path} {Status} — {Ms}ms",
                requestId, method, path, status, (int)ms);
        }
    }
}
