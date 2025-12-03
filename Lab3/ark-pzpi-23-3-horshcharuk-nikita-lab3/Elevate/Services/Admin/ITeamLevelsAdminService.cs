using Elevate.Entities;

namespace Elevate.Services.Admin;

public interface ITeamLevelsAdminService
{
    Task<IReadOnlyList<TeamLevel>> GetTeamLevelsAsync(int teamId, CancellationToken ct = default);
    Task<TeamLevel> CreateTeamLevelAsync(int teamId, string name, int requiredPoints, int orderIndex, CancellationToken ct = default);
    Task UpdateTeamLevelAsync(int levelId, string name, int requiredPoints, int orderIndex, CancellationToken ct = default);
    Task DeleteTeamLevelAsync(int levelId, CancellationToken ct = default);
}

