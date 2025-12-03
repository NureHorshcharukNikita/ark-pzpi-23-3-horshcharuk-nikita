using Elevate.Entities;

namespace Elevate.Services.Gamification;

public interface IGamificationService
{
    Task<IReadOnlyCollection<UserTeamBadge>> EvaluateBadgesAsync(
        int teamId,
        int userId,
        CancellationToken cancellationToken);
}
