namespace Elevate.Dtos.Users;

public class UserTeamInfoDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public int TeamPoints { get; set; }
    public string? TeamLevelName { get; set; }
    public IReadOnlyList<string> Badges { get; set; } = Array.Empty<string>();
}

