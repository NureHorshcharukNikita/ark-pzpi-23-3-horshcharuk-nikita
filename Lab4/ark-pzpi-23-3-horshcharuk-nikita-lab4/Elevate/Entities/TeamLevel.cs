namespace Elevate.Entities;

public class TeamLevel
{
    public int TeamLevelID { get; set; }
    public int TeamID { get; set; }
    public string Name { get; set; } = null!;
    public int RequiredPoints { get; set; }
    public int OrderIndex { get; set; }

    public Team Team { get; set; } = null!;
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}

