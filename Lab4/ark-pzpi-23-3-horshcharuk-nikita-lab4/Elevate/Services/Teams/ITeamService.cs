using Elevate.Dtos.Actions;
using Elevate.Dtos.Teams;

public interface ITeamService
{
    Task<IReadOnlyCollection<TeamDto>> GetTeamsAsync(CancellationToken cancellationToken);
    Task<TeamDetailDto?> GetTeamAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TeamMemberDto>> GetMembersAsync(int teamId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LeaderboardEntryDto>> GetLeaderboardAsync(
        int teamId,
        CancellationToken cancellationToken,
        int top = 10);
    Task<TeamLevelDto> CreateLevelAsync(int teamId, CreateTeamLevelDto dto, CancellationToken cancellationToken);
    Task<TeamBadgeDto> CreateBadgeAsync(int teamId, CreateTeamBadgeDto dto, CancellationToken cancellationToken);
    Task<ActionTypeDto> CreateActionTypeAsync(int teamId, CreateActionTypeDto dto, CancellationToken cancellationToken);
}
