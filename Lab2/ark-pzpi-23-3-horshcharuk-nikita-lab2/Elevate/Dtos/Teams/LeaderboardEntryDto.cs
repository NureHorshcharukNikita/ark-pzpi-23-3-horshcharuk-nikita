namespace Elevate.Dtos.Teams;

public class LeaderboardEntryDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public int TeamPoints { get; set; }
    public string? TeamLevel { get; set; }
    public int Rank { get; set; }
}
