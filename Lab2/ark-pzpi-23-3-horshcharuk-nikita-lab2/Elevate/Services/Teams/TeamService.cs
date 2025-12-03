using Elevate.Data;
using Elevate.Dtos.Actions;
using Elevate.Dtos.Teams;
using Elevate.Entities;
using Elevate.Mappings.Teams;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Teams;

public class TeamService : ITeamService
{
    private readonly ElevateDbContext _dbContext;

    public TeamService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyCollection<TeamDto>> GetTeamsAsync(CancellationToken cancellationToken)
    {
        var teams = await _dbContext.Teams
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TeamDto
            {
                Id = t.TeamID,
                Name = t.Name,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return teams;
    }

    public async Task<TeamDetailDto?> GetTeamAsync(int id, CancellationToken cancellationToken)
    {
        var team = await GetTeamWithDetailsQuery()
            .FirstOrDefaultAsync(t => t.TeamID == id, cancellationToken);

        if (team is null)
        {
            return null;
        }

        return new TeamDetailDto
        {
            Id = team.TeamID,
            Name = team.Name,
            Description = team.Description,
            CreatedAt = team.CreatedAt,
            Members = team.Members
                .Select(TeamMappings.ToTeamMemberDto)
                .ToArray(),
            Levels = team.Levels
                .OrderBy(tl => tl.OrderIndex)
                .Select(TeamMappings.ToTeamLevelDto)
                .ToArray(),
            Badges = team.Badges
                .Select(TeamMappings.ToTeamBadgeDto)
                .ToArray()
        };
    }

    public async Task<IReadOnlyCollection<TeamMemberDto>> GetMembersAsync(
        int teamId,
        CancellationToken cancellationToken)
    {
        var members = await _dbContext.TeamMembers
            .AsNoTracking()
            .Where(tm => tm.TeamID == teamId)
            .Include(tm => tm.User)
            .Include(tm => tm.TeamLevel)
            .OrderByDescending(tm => tm.TeamPoints)
            .ToListAsync(cancellationToken);

        return members
            .Select(TeamMappings.ToTeamMemberDto)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<LeaderboardEntryDto>> GetLeaderboardAsync(
        int teamId,
        CancellationToken cancellationToken,
        int top = 10)
    {
        var members = await _dbContext.TeamMembers
            .AsNoTracking()
            .Where(tm => tm.TeamID == teamId)
            .Include(tm => tm.User)
            .Include(tm => tm.TeamLevel)
            .OrderByDescending(tm => tm.TeamPoints)
            .Take(top)
            .ToListAsync(cancellationToken);

        var leaderboard = members
            .Select((tm, index) => TeamMappings.ToLeaderboardEntryDto(tm, index + 1))
            .ToArray();

        return leaderboard;
    }

    public async Task<TeamLevelDto> CreateLevelAsync(
        int teamId,
        CreateTeamLevelDto dto,
        CancellationToken cancellationToken)
    {
        await EnsureTeamExistsAsync(teamId, cancellationToken);

        var level = new TeamLevel
        {
            TeamID = teamId,
            Name = dto.Name,
            RequiredPoints = dto.RequiredPoints,
            OrderIndex = dto.OrderIndex
        };

        _dbContext.TeamLevels.Add(level);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return TeamMappings.ToTeamLevelDto(level);
    }

    public async Task<TeamBadgeDto> CreateBadgeAsync(
        int teamId,
        CreateTeamBadgeDto dto,
        CancellationToken cancellationToken)
    {
        await EnsureTeamExistsAsync(teamId, cancellationToken);

        var badge = new TeamBadge
        {
            TeamID = teamId,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            IconCode = dto.IconCode,
            ConditionType = dto.ConditionType,
            ConditionValue = dto.ConditionValue
        };

        _dbContext.TeamBadges.Add(badge);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return TeamMappings.ToTeamBadgeDto(badge);
    }

    public async Task<ActionTypeDto> CreateActionTypeAsync(
        int teamId,
        CreateActionTypeDto dto,
        CancellationToken cancellationToken)
    {
        await EnsureTeamExistsAsync(teamId, cancellationToken);

        var actionType = new ActionType
        {
            TeamID = teamId,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            DefaultPoints = dto.DefaultPoints,
            Category = dto.Category,
            IsActive = dto.IsActive
        };

        _dbContext.ActionTypes.Add(actionType);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ActionTypeDto
        {
            Id = actionType.ActionTypeID,
            TeamId = actionType.TeamID,
            Code = actionType.Code,
            Name = actionType.Name,
            Description = actionType.Description,
            DefaultPoints = actionType.DefaultPoints,
            Category = actionType.Category,
            IsActive = actionType.IsActive
        };
    }

    private IQueryable<Team> GetTeamWithDetailsQuery() =>
        _dbContext.Teams
            .AsNoTracking()
            .Include(t => t.Members)
                .ThenInclude(tm => tm.User)
            .Include(t => t.Members)
                .ThenInclude(tm => tm.TeamLevel)
            .Include(t => t.Levels)
            .Include(t => t.Badges);

    private Task<bool> TeamExistsAsync(int teamId, CancellationToken cancellationToken) =>
        _dbContext.Teams
            .AsNoTracking()
            .AnyAsync(t => t.TeamID == teamId, cancellationToken);

    private async Task EnsureTeamExistsAsync(int teamId, CancellationToken cancellationToken)
    {
        if (!await TeamExistsAsync(teamId, cancellationToken))
        {
            throw new InvalidOperationException($"Team with id {teamId} not found.");
        }
    }
}
