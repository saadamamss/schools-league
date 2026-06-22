using Common;
using Common.Models;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Shifts;

public class ShiftsService
{
    private readonly AppDbContext _db;
    public ShiftsService(AppDbContext db) { _db = db; }

    public async Task<PaginatedResult<ShiftDto>> GetAllAsync(int page, int perPage, string? search, string? status)
    {
        var query = _db.Shifts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(s => s.Name.Contains(search));
        if (status == "active") query = query.Where(s => s.IsActive);
        else if (status == "inactive") query = query.Where(s => !s.IsActive);

        var total = await query.CountAsync();
        var shifts = await query.OrderBy(s => s.Name).Skip((page - 1) * perPage).Take(perPage).ToListAsync();

        return new PaginatedResult<ShiftDto>
        {
            Data = shifts.Select(s => new ShiftDto { Id = s.Id, Name = s.Name, StartTime = s.StartTime.ToString(@"hh\:mm"), EndTime = s.EndTime.ToString(@"hh\:mm"), LocationId = s.LocationId, IsActive = s.IsActive }).ToList(),
            TotalObjects = total, PerPage = perPage, CurrentPage = page,
        };
    }

    public async Task<Result<ShiftDto>> CreateAsync(CreateShiftDto dto)
    {
        var shift = new Shift
        {
            Name = dto.Name,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            LocationId = dto.LocationId,
            IsActive = dto.IsActive,
        };
        _db.Shifts.Add(shift);
        await _db.SaveChangesAsync();
        var dtoOut = new ShiftDto { Id = shift.Id, Name = shift.Name, StartTime = shift.StartTime.ToString(@"hh\:mm"), EndTime = shift.EndTime.ToString(@"hh\:mm"), LocationId = shift.LocationId, IsActive = shift.IsActive };
        return Result<ShiftDto>.Ok(dtoOut, "Shift created successfully.", AppCodes.Created);
    }

    public async Task<Result<ShiftDto>> UpdateAsync(int id, UpdateShiftDto dto)
    {
        var shift = await _db.Shifts.FindAsync(id);
        if (shift == null) return Result<ShiftDto>.NotFound("Shift not found.");
        if (dto.Name != null) shift.Name = dto.Name;
        if (dto.StartTime.HasValue) shift.StartTime = dto.StartTime.Value;
        if (dto.EndTime.HasValue) shift.EndTime = dto.EndTime.Value;
        if (dto.LocationId.HasValue) shift.LocationId = dto.LocationId;
        if (dto.IsActive.HasValue) shift.IsActive = dto.IsActive.Value;
        await _db.SaveChangesAsync();
        var dtoOut = new ShiftDto { Id = shift.Id, Name = shift.Name, StartTime = shift.StartTime.ToString(@"hh\:mm"), EndTime = shift.EndTime.ToString(@"hh\:mm"), LocationId = shift.LocationId, IsActive = shift.IsActive };
        return Result<ShiftDto>.Ok(dtoOut, "Shift updated successfully.");
    }
}
