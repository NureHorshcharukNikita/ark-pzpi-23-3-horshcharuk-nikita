namespace Elevate.Dtos.Teams;

public class CreateTeamBadgeDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconCode { get; set; }
    public string? ConditionType { get; set; }
    public int? ConditionValue { get; set; }
}
