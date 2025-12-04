namespace Elevate.Entities;

public class TeamMember
{
    public int TeamMemberID { get; set; }
    public int TeamID { get; set; }
    public int UserID { get; set; }
    public string TeamRole { get; set; } = "Member";
    public int? TeamLevelID { get; set; }
    public int TeamPoints { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
    public TeamLevel? TeamLevel { get; set; }
}

