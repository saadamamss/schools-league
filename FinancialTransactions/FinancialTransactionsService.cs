using Common;
using Common.Models;
using Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialTransactions;

public class FinancialTransactionsService
{
    private readonly AppDbContext _db;
    public FinancialTransactionsService(AppDbContext db) { _db = db; }

    public async Task<PaginatedResult<FinancialTransactionDto>> GetByUserAsync(int userId, int page, int perPage)
    {
        var query = _db.FinancialTransactions
            .Include(ft => ft.PaymentType)
            .Where(ft => ft.UserId == userId);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(ft => ft.TransactionDate).Skip((page - 1) * perPage).Take(perPage).ToListAsync();
        return new PaginatedResult<FinancialTransactionDto>
        {
            Data = items.Select(MapToDto).ToList(),
            TotalObjects = total, PerPage = perPage, CurrentPage = page,
        };
    }

    public async Task<PaginatedResult<FinancialTransactionDto>> GetAllAsync(int page, int perPage)
    {
        var query = _db.FinancialTransactions
            .Include(ft => ft.PaymentType)
            .Include(ft => ft.User);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(ft => ft.TransactionDate).Skip((page - 1) * perPage).Take(perPage).ToListAsync();
        return new PaginatedResult<FinancialTransactionDto>
        {
            Data = items.Select(MapToDto).ToList(),
            TotalObjects = total, PerPage = perPage, CurrentPage = page,
        };
    }

    public async Task<Result<FinancialTransactionDto>> CreateAsync(CreateFinancialTransactionDto dto)
    {
        if (!await _db.Users.AnyAsync(u => u.Id == dto.UserId))
            return Result<FinancialTransactionDto>.NotFound("User not found.");
        if (dto.PaymentTypeId.HasValue && !await _db.PaymentTypes.AnyAsync(pt => pt.Id == dto.PaymentTypeId))
            return Result<FinancialTransactionDto>.NotFound("Payment type not found.");

        var transaction = new FinancialTransaction
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            Type = dto.Type,
            Description = dto.Description,
            PaymentTypeId = dto.PaymentTypeId,
            DeductionBasis = dto.DeductionBasis,
            DeductionValue = dto.DeductionValue,
        };

        _db.FinancialTransactions.Add(transaction);
        await _db.SaveChangesAsync();

        var dtoOut = MapToDto(transaction);
        return Result<FinancialTransactionDto>.Ok(dtoOut, "Transaction created successfully.", AppCodes.Created);
    }

    private static FinancialTransactionDto MapToDto(FinancialTransaction ft) => new()
    {
        Id = ft.Id,
        UserId = ft.UserId,
        Amount = ft.Amount,
        Type = ft.Type,
        Description = ft.Description,
        PaymentTypeId = ft.PaymentTypeId,
        DeductionBasis = ft.DeductionBasis,
        DeductionValue = ft.DeductionValue,
        TransactionDate = ft.TransactionDate,
        User = ft.User != null
            ? new UserSummaryDto { Id = ft.User.Id, FullName = ft.User.FullName, Email = ft.User.Email!, Phone = ft.User.PhoneNumber!, FirstName = ft.User.FirstName, LastName = ft.User.LastName }
            : null,
        PaymentType = ft.PaymentType != null
            ? new PaymentTypeDto { Id = ft.PaymentType.Id, Name = ft.PaymentType.Name }
            : null,
    };
}
