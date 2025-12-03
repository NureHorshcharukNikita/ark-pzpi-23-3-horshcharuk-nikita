using Elevate.Entities;

namespace Elevate.Services.Admin;

public interface IAdminUserService
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<User?> GetByIdAsync(int userId, CancellationToken ct = default);
    Task SetRoleAsync(int userId, string role, CancellationToken ct = default);
    Task SetActiveAsync(int userId, bool isActive, CancellationToken ct = default);
}

