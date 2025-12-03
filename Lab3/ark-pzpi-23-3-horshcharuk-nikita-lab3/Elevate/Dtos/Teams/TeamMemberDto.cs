namespace Elevate.Dtos.Teams;

public class TeamMemberDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string TeamRole { get; set; } = null!;
    public int TeamPoints { get; set; }
    public string? TeamLevel { get; set; }
}
