namespace Elevate.Dtos.IoT;

public class IotScanResultDto
{
    public int UserId { get; set; }
    public int TeamId { get; set; }
    public string FullName { get; set; } = null!;
    public int TeamPoints { get; set; }
    public string? TeamLevelName { get; set; }
    public IReadOnlyList<string> RecentBadges { get; set; } = Array.Empty<string>();
}

