namespace Elevate.Dtos.Actions;

public class ActionEventResultDto
{
    public int ActionEventId { get; set; }
    public int UserId { get; set; }
    public int TeamId { get; set; }
    public int PointsAwarded { get; set; }
    public int TotalTeamPoints { get; set; }
    public string? NewTeamLevelName { get; set; }
    public IReadOnlyList<string> NewBadges { get; set; } = Array.Empty<string>();
}

