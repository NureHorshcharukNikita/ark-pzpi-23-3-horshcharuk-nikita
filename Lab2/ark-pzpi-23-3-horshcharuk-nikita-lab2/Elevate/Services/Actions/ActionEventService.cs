using Elevate.Data;
using Elevate.Dtos.Actions;
using Elevate.Entities;
using Elevate.Mappings.Actions;
using Elevate.Services.Gamification;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Actions;

public class ActionEventService : IActionEventService
{
    private readonly ElevateDbContext _dbContext;
    private readonly IGamificationService _gamificationService;

    public ActionEventService(
        ElevateDbContext dbContext,
        IGamificationService gamificationService)
    {
        _dbContext = dbContext;
        _gamificationService = gamificationService;
    }

    public async Task<ActionEventDto> CreateAsync(
        CreateActionEventDto dto,
        int? authenticatedUserId,
        CancellationToken cancellationToken)
    {
        var userId = ResolveUserId(dto, authenticatedUserId);

        var actionType = await GetValidActionTypeAsync(dto, cancellationToken);
        var membership = await GetMembershipAsync(userId, dto.TeamId, cancellationToken);

        var points = dto.Points ?? actionType.DefaultPoints;

        var actionEvent = CreateActionEventEntity(dto, userId, points);

        _dbContext.ActionEvents.Add(actionEvent);
        membership.TeamPoints += points;

        var newLevel = await UpdateMembershipLevelAsync(membership, dto.TeamId, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var earnedBadges = await _gamificationService.EvaluateBadgesAsync(
            dto.TeamId,
            userId,
            cancellationToken);

        var membershipLevelName = newLevel?.Name ?? membership.TeamLevel?.Name;

        var badgeLookup = earnedBadges
            .Select(b => b.TeamBadge)
            .Where(tb => tb != null)
            .Cast<TeamBadge>()
            .ToDictionary(tb => tb.TeamBadgeID, tb => tb.Name);

        return ActionEventMappings.ToActionEventDto(
            actionEvent,
            membership,
            membershipLevelName,
            earnedBadges,
            badgeLookup);
    }

    public async Task<IReadOnlyCollection<ActionEventDto>> GetAsync(
        int? userId,
        int? teamId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.ActionEvents.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(ae => ae.UserID == userId.Value);
        }

        if (teamId.HasValue)
        {
            query = query.Where(ae => ae.TeamID == teamId.Value);
        }

        var results = await query
            .AsNoTracking()
            .OrderByDescending(ae => ae.OccurredAt)
            .Take(100)
            .Select(ActionEventMappings.ToListItem)
            .ToListAsync(cancellationToken);

        return results;
    }

    public Task<ActionEvent?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _dbContext.ActionEvents
            .AsNoTracking()
            .Include(ae => ae.User)
            .Include(ae => ae.ActionType)
            .FirstOrDefaultAsync(ae => ae.ActionEventID == id, cancellationToken);

    private static int ResolveUserId(CreateActionEventDto dto, int? authenticatedUserId)
    {
        return dto.UserId
               ?? authenticatedUserId
               ?? throw new InvalidOperationException("Missing user context");
    }

    private async Task<ActionType> GetValidActionTypeAsync(
        CreateActionEventDto dto,
        CancellationToken cancellationToken)
    {
        var actionType = await _dbContext.ActionTypes
            .FirstOrDefaultAsync(
                at => at.ActionTypeID == dto.ActionTypeId && at.TeamID == dto.TeamId,
                cancellationToken);

        if (actionType == null || !actionType.IsActive)
        {
            throw new InvalidOperationException("Invalid action type for this team");
        }

        return actionType;
    }

    private async Task<TeamMember> GetMembershipAsync(
        int userId,
        int teamId,
        CancellationToken cancellationToken)
    {
        var membership = await _dbContext.TeamMembers
            .Include(tm => tm.TeamLevel)
            .FirstOrDefaultAsync(
                tm => tm.UserID == userId && tm.TeamID == teamId,
                cancellationToken);

        if (membership == null)
        {
            throw new InvalidOperationException("User is not a member of this team");
        }

        return membership;
    }

    private static ActionEvent CreateActionEventEntity(
        CreateActionEventDto dto,
        int userId,
        int points)
    {
        return new ActionEvent
        {
            UserID = userId,
            TeamID = dto.TeamId,
            ActionTypeID = dto.ActionTypeId,
            SourceType = dto.SourceType,
            SourceUserID = dto.SourceUserId,
            Comment = dto.Comment,
            PointsAwarded = points,
            OccurredAt = dto.OccurredAt ?? DateTime.UtcNow
        };
    }

    private async Task<TeamLevel?> UpdateMembershipLevelAsync(
        TeamMember membership,
        int teamId,
        CancellationToken cancellationToken)
    {
        var newLevel = await _dbContext.TeamLevels
            .Where(tl => tl.TeamID == teamId && membership.TeamPoints >= tl.RequiredPoints)
            .OrderByDescending(tl => tl.RequiredPoints)
            .FirstOrDefaultAsync(cancellationToken);

        if (newLevel != null)
        {
            membership.TeamLevelID = newLevel.TeamLevelID;
        }

        return newLevel;
    }
}
