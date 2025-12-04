namespace Elevate.Dtos.Analytics;

public class TeamAnalyticsDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public int TotalEvents { get; set; }

    public IReadOnlyDictionary<string, int> EventsByType { get; set; }
        = new Dictionary<string, int>();

    public IReadOnlyCollection<LevelDistributionDto> LevelDistribution { get; set; }
        = Array.Empty<LevelDistributionDto>();

    public IReadOnlyCollection<BadgeStatsDto> BadgeStats { get; set; }
        = Array.Empty<BadgeStatsDto>();
}
