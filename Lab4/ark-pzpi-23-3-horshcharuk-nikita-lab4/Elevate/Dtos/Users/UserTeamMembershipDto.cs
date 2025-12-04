namespace Elevate.Dtos.Users;

public class UserTeamMembershipDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public string TeamRole { get; set; } = null!;
    public int TeamPoints { get; set; }
    public string? TeamLevel { get; set; }
}
