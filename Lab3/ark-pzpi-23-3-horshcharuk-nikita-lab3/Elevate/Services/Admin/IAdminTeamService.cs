using Elevate.Entities;

namespace Elevate.Services.Admin;

public interface IAdminTeamService
{
    Task<Team> CreateTeamAsync(string name, string? description, int? managerUserId, CancellationToken ct = default);
    Task UpdateTeamAsync(int teamId, string name, string? description, CancellationToken ct = default);
    Task DeleteTeamAsync(int teamId, CancellationToken ct = default);

    Task AddMemberAsync(int teamId, int userId, string teamRole, CancellationToken ct = default);
    Task RemoveMemberAsync(int teamId, int userId, CancellationToken ct = default);
    Task ChangeMemberRoleAsync(int teamId, int userId, string teamRole, CancellationToken ct = default);
}

