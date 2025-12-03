namespace Elevate.Entities;

public class User
{
    public int UserID { get; set; }
    public string Login { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public ICollection<ActionEvent> ActionEvents { get; set; } = new List<ActionEvent>();
    public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();
    public ICollection<UserTeamBadge> UserTeamBadges { get; set; } = new List<UserTeamBadge>();
    public ICollection<DeviceScan> DeviceScans { get; set; } = new List<DeviceScan>();
}

