namespace Elevate.Dtos.Teams;

public class TeamLevelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int RequiredPoints { get; set; }
    public int OrderIndex { get; set; }
}
