using Elevate.Dtos.Teams;

namespace Elevate.Services.Leaderboard;

public interface ILeaderboardService
{
    Task<IReadOnlyList<TeamLeaderboardEntryDto>> GetTeamLeaderboardAsync(
        int teamId,
        int? topN = null,
        CancellationToken ct = default);
}

