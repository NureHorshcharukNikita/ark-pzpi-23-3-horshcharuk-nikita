using System.Security.Claims;
using Elevate.Data;
using Elevate.Dtos.Users;
using Elevate.Mappings.Users;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Users;

public class UserService : IUserService
{
    private readonly ElevateDbContext _dbContext;

    public UserService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileDto?> GetProfileAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.TeamMemberships)
                .ThenInclude(tm => tm.Team)
            .Include(u => u.TeamMemberships)
                .ThenInclude(tm => tm.TeamLevel)
            .Include(u => u.UserTeamBadges)
                .ThenInclude(utb => utb.Team)
            .Include(u => u.UserTeamBadges)
                .ThenInclude(utb => utb.TeamBadge)
            .FirstOrDefaultAsync(u => u.UserID == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return UserMappings.ToUserProfileDto(user);
    }

    public Task<UserProfileDto?> GetProfileAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var subject =
            principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        if (!int.TryParse(subject, out var userId))
        {
            return Task.FromResult<UserProfileDto?>(null);
        }

        return GetProfileAsync(userId, cancellationToken);
    }
}
