namespace Elevate.Entities;

public class UserTeamBadge
{
    public int UserTeamBadgeID { get; set; }
    public int UserID { get; set; }
    public int TeamBadgeID { get; set; }
    public int TeamID { get; set; }
    public DateTime AwardedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public TeamBadge TeamBadge { get; set; } = null!;
    public Team Team { get; set; } = null!;
}

