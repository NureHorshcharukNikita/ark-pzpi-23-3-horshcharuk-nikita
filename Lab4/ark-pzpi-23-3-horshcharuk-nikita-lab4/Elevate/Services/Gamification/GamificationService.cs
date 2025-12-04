using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Gamification;

public class GamificationService : IGamificationService
{
    private readonly ElevateDbContext _dbContext;

    public GamificationService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<UserTeamBadge>> EvaluateBadgesAsync(
        int teamId,
        int userId,
        CancellationToken cancellationToken)
    {
        var membership = await GetMembershipAsync(teamId, userId, cancellationToken);
        if (membership is null)
        {
            return Array.Empty<UserTeamBadge>();
        }

        var allBadges = await GetTeamBadgesAsync(teamId, cancellationToken);
        var existingBadgeIds = await GetExistingBadgeIdsAsync(teamId, userId, cancellationToken);

        var earnedBadges = new List<UserTeamBadge>();

        foreach (var badge in allBadges)
        {
            if (existingBadgeIds.Contains(badge.TeamBadgeID))
            {
                continue;
            }

            if (CanAwardBadge(badge, membership))
            {
                var userTeamBadge = CreateUserTeamBadge(badge, membership);
                earnedBadges.Add(userTeamBadge);
                _dbContext.UserTeamBadges.Add(userTeamBadge);
            }
        }

        if (earnedBadges.Count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return earnedBadges;
    }

    public async Task<TeamLevel?> UpdateMembershipLevelAsync(
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

    private Task<TeamMember?> GetMembershipAsync(
        int teamId,
        int userId,
        CancellationToken cancellationToken) =>
        _dbContext.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamID == teamId && tm.UserID == userId, cancellationToken);

    private Task<List<TeamBadge>> GetTeamBadgesAsync(
        int teamId,
        CancellationToken cancellationToken) =>
        _dbContext.TeamBadges
            .Where(tb => tb.TeamID == teamId)
            .ToListAsync(cancellationToken);

    private async Task<HashSet<int>> GetExistingBadgeIdsAsync(
        int teamId,
        int userId,
        CancellationToken cancellationToken)
    {
        var ids = await _dbContext.UserTeamBadges
            .Where(utb => utb.TeamID == teamId && utb.UserID == userId)
            .Select(utb => utb.TeamBadgeID)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    private static bool CanAwardBadge(TeamBadge badge, TeamMember membership)
    {
        if (!badge.ConditionType?.Equals("TotalPoints", StringComparison.OrdinalIgnoreCase) ?? true)
        {
            return false;
        }

        if (!badge.ConditionValue.HasValue)
        {
            return false;
        }

        return membership.TeamPoints >= badge.ConditionValue.Value;
    }

    private static UserTeamBadge CreateUserTeamBadge(TeamBadge badge, TeamMember membership)
    {
        return new UserTeamBadge
        {
            TeamBadgeID = badge.TeamBadgeID,
            TeamID = membership.TeamID,
            UserID = membership.UserID,
            TeamBadge = badge
        };
    }
}
