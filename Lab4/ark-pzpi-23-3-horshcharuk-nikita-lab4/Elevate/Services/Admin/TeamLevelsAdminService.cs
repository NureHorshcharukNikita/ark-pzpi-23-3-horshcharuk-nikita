using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Admin;

public class TeamLevelsAdminService : ITeamLevelsAdminService
{
    private readonly ElevateDbContext _dbContext;

    public TeamLevelsAdminService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<TeamLevel>> GetTeamLevelsAsync(int teamId, CancellationToken ct = default)
    {
        return await _dbContext.TeamLevels
            .Where(l => l.TeamID == teamId)
            .OrderBy(l => l.OrderIndex)
            .ToListAsync(ct);
    }

    public async Task<TeamLevel> CreateTeamLevelAsync(int teamId, string name, int requiredPoints, int orderIndex, CancellationToken ct = default)
    {
        var level = new TeamLevel
        {
            TeamID = teamId,
            Name = name,
            RequiredPoints = requiredPoints,
            OrderIndex = orderIndex
        };

        _dbContext.TeamLevels.Add(level);
        await _dbContext.SaveChangesAsync(ct);
        return level;
    }

    public async Task UpdateTeamLevelAsync(int levelId, string name, int requiredPoints, int orderIndex, CancellationToken ct = default)
    {
        var level = await _dbContext.TeamLevels.FirstOrDefaultAsync(l => l.TeamLevelID == levelId, ct)
                     ?? throw new InvalidOperationException("Level not found");

        level.Name = name;
        level.RequiredPoints = requiredPoints;
        level.OrderIndex = orderIndex;

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteTeamLevelAsync(int levelId, CancellationToken ct = default)
    {
        var level = await _dbContext.TeamLevels.FirstOrDefaultAsync(l => l.TeamLevelID == levelId, ct)
                     ?? throw new InvalidOperationException("Level not found");

        _dbContext.TeamLevels.Remove(level);
        await _dbContext.SaveChangesAsync(ct);
    }
}

