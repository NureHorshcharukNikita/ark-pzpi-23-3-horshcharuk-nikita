namespace Elevate.Dtos.Actions;

public class CreateActionEventDto
{
    public int? UserId { get; set; }
    public int TeamId { get; set; }
    public int ActionTypeId { get; set; }
    public string SourceType { get; set; } = null!;
    public int? SourceUserId { get; set; }
    public string? Comment { get; set; }
    public int? Points { get; set; }
    public DateTime? OccurredAt { get; set; }
}