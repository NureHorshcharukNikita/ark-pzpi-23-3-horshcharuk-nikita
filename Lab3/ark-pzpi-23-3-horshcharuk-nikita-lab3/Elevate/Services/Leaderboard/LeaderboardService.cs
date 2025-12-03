using Elevate.Data;
using Elevate.Dtos.Teams;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Leaderboard;

public class LeaderboardService : ILeaderboardService
{
    private readonly ElevateDbContext _dbContext;

    public LeaderboardService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<TeamLeaderboardEntryDto>> GetTeamLeaderboardAsync(
        int teamId,
        int? topN = null,
        CancellationToken ct = default)
    {
        var query = _dbContext.TeamMembers
            .AsNoTracking()
            .Include(tm => tm.User)
            .Include(tm => tm.TeamLevel)
            .Where(tm => tm.TeamID == teamId)
            .OrderByDescending(tm => tm.TeamPoints)
            .AsQueryable();

        if (topN.HasValue)
        {
            query = query.Take(topN.Value);
        }

        var members = await query.ToListAsync(ct);

        var result = new List<TeamLeaderboardEntryDto>();
        int position = 1;

        foreach (var tm in members)
        {
            result.Add(new TeamLeaderboardEntryDto
            {
                Position = position++,
                UserId = tm.UserID,
                FullName = $"{tm.User.FirstName} {tm.User.LastName}",
                TeamPoints = tm.TeamPoints,
                TeamLevelName = tm.TeamLevel?.Name
            });
        }

        return result;
    }
}

