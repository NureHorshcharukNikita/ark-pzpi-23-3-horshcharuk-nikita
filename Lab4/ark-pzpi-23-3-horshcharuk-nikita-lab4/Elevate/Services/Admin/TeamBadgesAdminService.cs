using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Admin;

public class TeamBadgesAdminService : ITeamBadgesAdminService
{
    private readonly ElevateDbContext _dbContext;

    public TeamBadgesAdminService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<TeamBadge>> GetTeamBadgesAsync(int teamId, CancellationToken ct = default)
    {
        return await _dbContext.TeamBadges
            .Where(b => b.TeamID == teamId)
            .OrderBy(b => b.Code)
            .ToListAsync(ct);
    }

    public async Task<TeamBadge> CreateTeamBadgeAsync(int teamId, string code, string name, string description,
        string iconCode, string conditionType, int conditionValue, CancellationToken ct = default)
    {
        var badge = new TeamBadge
        {
            TeamID = teamId,
            Code = code,
            Name = name,
            Description = description,
            IconCode = iconCode,
            ConditionType = conditionType,
            ConditionValue = conditionValue
        };

        _dbContext.TeamBadges.Add(badge);
        await _dbContext.SaveChangesAsync(ct);
        return badge;
    }

    public async Task UpdateTeamBadgeAsync(int badgeId, string name, string description, string iconCode,
        string conditionType, int conditionValue, CancellationToken ct = default)
    {
        var badge = await _dbContext.TeamBadges.FirstOrDefaultAsync(b => b.TeamBadgeID == badgeId, ct)
                    ?? throw new InvalidOperationException("Badge not found");

        badge.Name = name;
        badge.Description = description;
        badge.IconCode = iconCode;
        badge.ConditionType = conditionType;
        badge.ConditionValue = conditionValue;

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteTeamBadgeAsync(int badgeId, CancellationToken ct = default)
    {
        var badge = await _dbContext.TeamBadges.FirstOrDefaultAsync(b => b.TeamBadgeID == badgeId, ct)
                    ?? throw new InvalidOperationException("Badge not found");

        _dbContext.TeamBadges.Remove(badge);
        await _dbContext.SaveChangesAsync(ct);
    }
}

