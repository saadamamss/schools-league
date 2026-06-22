using FinancialTransactions;
using Common;
using Common.Models;
using Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FinancialTransactionsNamespace;

[ApiController]
[Route("api/dashboard/financial-transactions")]
[SwaggerTag("Financial Transactions")]
[Authorize]
public class FinancialTransactionsController : BaseController
{
    private readonly FinancialTransactionsService _service;

    public FinancialTransactionsController(FinancialTransactionsService service) { _service = service; }

    [HttpGet("user/{userId}")]
    [SwaggerOperation("Get transactions for a user")]
    [ProducesResponseType(typeof(ApiResponse<FinancialTransactionDto>), 200)]
    public async Task<IActionResult> GetByUser(int userId, [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int per_page = 10)
    {
        var result = await _service.GetByUserAsync(userId, page, per_page);
        return OkPaginated(result.Data, result.TotalObjects, result.PerPage, result.CurrentPage);
    }

    [HttpGet]
    [SwaggerOperation("Get all transactions (paginated)")]
    [ProducesResponseType(typeof(ApiResponse<FinancialTransactionDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery][Range(1, 100)] int per_page = 10)
    {
        var result = await _service.GetAllAsync(page, per_page);
        return OkPaginated(result.Data, result.TotalObjects, result.PerPage, result.CurrentPage);
    }

    [HttpPost]
    [SwaggerOperation("Create a transaction")]
    [ProducesResponseType(typeof(ApiResponse<FinancialTransactionDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateFinancialTransactionDto dto) =>
        (await _service.CreateAsync(dto)).ToActionResult();
}
