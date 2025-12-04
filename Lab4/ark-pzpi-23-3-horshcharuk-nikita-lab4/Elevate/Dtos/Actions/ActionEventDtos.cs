namespace Elevate.Dtos.Actions;

public class ActionEventDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TeamId { get; set; }
    public int ActionTypeId { get; set; }
    public string SourceType { get; set; } = null!;
    public int PointsAwarded { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TeamPoints { get; set; }
    public string? TeamLevel { get; set; }
    public IReadOnlyCollection<EarnedTeamBadgeDto> NewTeamBadges { get; set; }
        = Array.Empty<EarnedTeamBadgeDto>();
}
