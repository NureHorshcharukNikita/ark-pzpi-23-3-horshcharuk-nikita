namespace Elevate.Dtos.Teams;

public class TeamLeaderboardEntryDto
{
    public int Position { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public int TeamPoints { get; set; }
    public string? TeamLevelName { get; set; }
}

