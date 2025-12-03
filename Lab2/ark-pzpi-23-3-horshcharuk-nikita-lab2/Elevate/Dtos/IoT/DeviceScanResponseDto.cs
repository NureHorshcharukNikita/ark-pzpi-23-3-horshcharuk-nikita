namespace Elevate.Dtos.IoT;

public class DeviceScanResponseDto
{
    public string FullName { get; set; } = null!;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public int TeamPoints { get; set; }
    public string? TeamLevel { get; set; }
    public IReadOnlyCollection<string> RecentBadges { get; set; }
        = Array.Empty<string>();
}
