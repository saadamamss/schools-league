using Common;
using Common.Models;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Locations;

public class UserLocationAssignmentsService
{
    private readonly AppDbContext _db;

    public UserLocationAssignmentsService(AppDbContext db) { _db = db; }

    public async Task<PaginatedResult<UserLocationAssignmentDto>> GetAllAsync(int page, int perPage, int? userId, int? locationId)
    {
        var query = _db.UserLocationAssignments
            .Include(ula => ula.User)
            .Include(ula => ula.Location)
            .AsQueryable();

        if (userId.HasValue) query = query.Where(ula => ula.UserId == userId);
        if (locationId.HasValue) query = query.Where(ula => ula.LocationId == locationId);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(ula => ula.AssignedAt).Skip((page - 1) * perPage).Take(perPage).ToListAsync();

        return new PaginatedResult<UserLocationAssignmentDto>
        {
            Data = items.Select(MapToDto).ToList(),
            TotalObjects = total, PerPage = perPage, CurrentPage = page,
        };
    }

    public async Task<Result<UserLocationAssignmentDto>> CreateAsync(CreateUserLocationAssignmentDto dto)
    {
        if (await _db.UserLocationAssignments.AnyAsync(ula => ula.UserId == dto.UserId && ula.LocationId == dto.LocationId && !ula.IsDeleted))
            return Result<UserLocationAssignmentDto>.Conflict("User is already assigned to this location.");
        if (!await _db.Users.AnyAsync(u => u.Id == dto.UserId))
            return Result<UserLocationAssignmentDto>.NotFound("User not found.");
        if (!await _db.Locations.AnyAsync(l => l.Id == dto.LocationId))
            return Result<UserLocationAssignmentDto>.NotFound("Location not found.");

        var assignment = new UserLocationAssignment
        {
            UserId = dto.UserId,
            LocationId = dto.LocationId,
            Role = dto.Role,
        };
        _db.UserLocationAssignments.Add(assignment);
        await _db.SaveChangesAsync();

        var dtoOut = MapToDto(assignment);
        return Result<UserLocationAssignmentDto>.Ok(dtoOut, "Assignment created successfully.");
    }

    public async Task<Result<UserLocationAssignmentDto>> UpdateAsync(int id, UpdateUserLocationAssignmentDto dto)
    {
        var assignment = await _db.UserLocationAssignments.FindAsync(id);
        if (assignment == null) return Result<UserLocationAssignmentDto>.NotFound("Assignment not found.");
        if (dto.Role != null) assignment.Role = dto.Role;
        await _db.SaveChangesAsync();
        return Result<UserLocationAssignmentDto>.Ok(MapToDto(assignment), "Assignment updated successfully.");
    }

    private static UserLocationAssignmentDto MapToDto(UserLocationAssignment ula) => new()
    {
        Id = ula.Id, UserId = ula.UserId, LocationId = ula.LocationId,
        Role = ula.Role, AssignedAt = ula.AssignedAt,
    };
}
