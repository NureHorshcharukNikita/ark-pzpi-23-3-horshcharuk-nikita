using Elevate.Data;
using Elevate.Dtos.Users;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Users;

public class UserProfileService : IUserProfileService
{
    private readonly ElevateDbContext _dbContext;

    public UserProfileService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<UserTeamsProfileDto> GetUserTeamsProfileAsync(int userId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserID == userId, ct)
            ?? throw new InvalidOperationException("User not found");

        var memberships = await _dbContext.TeamMembers
            .AsNoTracking()
            .Include(tm => tm.Team)
            .Include(tm => tm.TeamLevel)
            .Where(tm => tm.UserID == userId)
            .ToListAsync(ct);

        var userBadges = await _dbContext.UserTeamBadges
            .AsNoTracking()
            .Include(utb => utb.TeamBadge)
            .Where(utb => utb.UserID == userId)
            .ToListAsync(ct);

        var teamInfos = memberships.Select(tm =>
        {
            var badgesForTeam = userBadges
                .Where(utb => utb.TeamID == tm.TeamID)
                .Select(utb => utb.TeamBadge.Name)
                .ToList();

            return new UserTeamInfoDto
            {
                TeamId = tm.TeamID,
                TeamName = tm.Team.Name,
                TeamPoints = tm.TeamPoints,
                TeamLevelName = tm.TeamLevel?.Name,
                Badges = badgesForTeam
            };
        }).ToList();

        return new UserTeamsProfileDto
        {
            UserId = user.UserID,
            FullName = $"{user.FirstName} {user.LastName}",
            Teams = teamInfos
        };
    }
}

