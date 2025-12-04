using Elevate.Data;
using Elevate.Dtos.Analytics;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Analytics;

public class AnalyticsService : IAnalyticsService
{
    private readonly ElevateDbContext _dbContext;

    public AnalyticsService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TeamAnalyticsDto?> GetOverviewAsync(
        int teamId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken)
    {
        var team = await _dbContext.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TeamID == teamId, cancellationToken);

        if (team is null)
        {
            return null;
        }

        return new TeamAnalyticsDto
        {
            TeamId = team.TeamID,
            TeamName = team.Name,
            TotalEvents = 0,
            EventsByType = new Dictionary<string, int>(),
            LevelDistribution = new List<LevelDistributionDto>(),
            BadgeStats = new List<BadgeStatsDto>()
        };
    }
}
