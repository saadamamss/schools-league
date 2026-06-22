using Common;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace HealthNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Health")]
public class HealthController : BaseController
{
    private readonly AppDbContext _db;

    public HealthController(AppDbContext db) { _db = db; }

    [HttpGet]
    [SwaggerOperation("Basic health check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => OkResult(new { status = "healthy", timestamp = DateTime.UtcNow });

    [HttpGet("ready")]
    [SwaggerOperation("Readiness check (DB connection)")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Ready()
    {
        try
        {
            await _db.Database.CanConnectAsync();
            return OkResult(new { status = "ready", database = "connected" });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { status = "unhealthy", database = ex.Message });
        }
    }
}
