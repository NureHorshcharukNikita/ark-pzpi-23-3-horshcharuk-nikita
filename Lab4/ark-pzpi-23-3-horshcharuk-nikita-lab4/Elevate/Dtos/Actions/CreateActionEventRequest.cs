namespace Elevate.Dtos.Actions;

public record CreateActionEventRequest(
    int UserId,
    int ActionTypeId,
    string SourceType,
    int? SourceUserId,
    string? Comment,
    DateTime? OccurredAt);

