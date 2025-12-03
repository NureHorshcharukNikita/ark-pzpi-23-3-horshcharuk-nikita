using Elevate.Entities;

namespace Elevate.Services.Admin;

public interface ITeamBadgesAdminService
{
    Task<IReadOnlyList<TeamBadge>> GetTeamBadgesAsync(int teamId, CancellationToken ct = default);
    Task<TeamBadge> CreateTeamBadgeAsync(int teamId, string code, string name, string description,
        string iconCode, string conditionType, int conditionValue, CancellationToken ct = default);
    Task UpdateTeamBadgeAsync(int badgeId, string name, string description, string iconCode,
        string conditionType, int conditionValue, CancellationToken ct = default);
    Task DeleteTeamBadgeAsync(int badgeId, CancellationToken ct = default);
}

