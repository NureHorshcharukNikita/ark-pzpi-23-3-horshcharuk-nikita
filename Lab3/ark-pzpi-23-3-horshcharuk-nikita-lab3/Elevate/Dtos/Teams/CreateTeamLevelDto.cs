namespace Elevate.Dtos.Teams;

public class CreateTeamLevelDto
{
    public string Name { get; set; } = null!;
    public int RequiredPoints { get; set; }
    public int OrderIndex { get; set; }
}
