using Common;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace ReferenceNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Reference Data")]
[Authorize]
public class ReferenceController : BaseController
{
    private readonly AppDbContext _db;

    public ReferenceController(AppDbContext db) { _db = db; }

    [HttpGet("cities")]
    [SwaggerOperation("Get all cities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCities()
    {
        try
        {
            var cities = await _db.Cities.Select(c => new { c.Id, Name = new { ar = c.NameAr, en = c.NameEn } }).ToListAsync();
            return OkResult(cities);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve cities");
            return ErrorResult("Failed to retrieve cities");
        }
    }

    [HttpGet("user-types")]
    [SwaggerOperation("Get all user types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserTypes()
    {
        try
        {
            var types = await _db.UserTypes.Select(ut => new { ut.Id, ut.Code, Name = new { ar = ut.NameAr, en = ut.NameEn }, ut.Key, ut.IsActive }).ToListAsync();
            return OkResult(types);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve user types");
            return ErrorResult("Failed to retrieve user types");
        }
    }

    [HttpGet("nationalities")]
    [SwaggerOperation("Get all nationalities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNationalities()
    {
        try
        {
            var nationalities = await _db.Nationalities.Select(n => new { n.Id, Name = new { ar = n.NameAr, en = n.NameEn }, n.Code }).ToListAsync();
            return OkResult(nationalities);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve nationalities");
            return ErrorResult("Failed to retrieve nationalities");
        }
    }

    [HttpGet("payment-types")]
    [SwaggerOperation("Get all payment types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaymentTypes()
    {
        try
        {
            var types = await _db.PaymentTypes.ToListAsync();
            return OkResult(types);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve payment types");
            return ErrorResult("Failed to retrieve payment types");
        }
    }
}
