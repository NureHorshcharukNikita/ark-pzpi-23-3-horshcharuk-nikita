using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Admin;

public class AdminUserService : IAdminUserService
{
    private readonly ElevateDbContext _dbContext;

    public AdminUserService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.Users
            .OrderBy(u => u.UserID)
            .ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(int userId, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserID == userId, ct);
    }

    public async Task SetRoleAsync(int userId, string role, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == userId, ct)
                   ?? throw new InvalidOperationException("User not found");

        if (role is not ("User" or "Manager" or "Admin"))
            throw new InvalidOperationException("Invalid role");

        user.Role = role;
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task SetActiveAsync(int userId, bool isActive, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == userId, ct)
                   ?? throw new InvalidOperationException("User not found");

        user.IsActive = isActive;
        await _dbContext.SaveChangesAsync(ct);
    }
}

