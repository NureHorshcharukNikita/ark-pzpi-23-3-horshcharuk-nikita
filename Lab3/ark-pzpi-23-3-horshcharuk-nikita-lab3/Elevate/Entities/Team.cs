namespace Elevate.Entities;

public class Team
{
    public int TeamID { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
    public ICollection<TeamLevel> Levels { get; set; } = new List<TeamLevel>();
    public ICollection<TeamBadge> Badges { get; set; } = new List<TeamBadge>();
    public ICollection<UserTeamBadge> UserTeamBadges { get; set; } = new List<UserTeamBadge>();
    public ICollection<ActionType> ActionTypes { get; set; } = new List<ActionType>();
    public ICollection<ActionEvent> ActionEvents { get; set; } = new List<ActionEvent>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
    public ICollection<DeviceScan> DeviceScans { get; set; } = new List<DeviceScan>();
}

