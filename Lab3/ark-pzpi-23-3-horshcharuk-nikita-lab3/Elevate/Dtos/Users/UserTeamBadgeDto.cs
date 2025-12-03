namespace Elevate.Dtos.Users;

public class UserTeamBadgeDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public string BadgeName { get; set; } = null!;
    public DateTime AwardedAt { get; set; }
}
