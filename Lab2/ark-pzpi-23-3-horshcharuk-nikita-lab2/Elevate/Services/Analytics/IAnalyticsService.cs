using Elevate.Dtos.Analytics;

namespace Elevate.Services.Analytics;

public interface IAnalyticsService
{
    Task<TeamAnalyticsDto?> GetOverviewAsync(
        int teamId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken);
}
