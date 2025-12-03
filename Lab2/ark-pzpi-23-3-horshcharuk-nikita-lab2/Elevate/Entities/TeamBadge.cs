namespace Elevate.Entities;

public class TeamBadge
{
    public int TeamBadgeID { get; set; }
    public int TeamID { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconCode { get; set; }
    public string? ConditionType { get; set; }
    public int? ConditionValue { get; set; }

    public Team Team { get; set; } = null!;
    public ICollection<UserTeamBadge> UserTeamBadges { get; set; } = new List<UserTeamBadge>();
}

