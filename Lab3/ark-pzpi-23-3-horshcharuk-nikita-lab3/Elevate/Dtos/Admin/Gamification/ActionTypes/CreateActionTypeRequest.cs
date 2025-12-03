namespace Elevate.Dtos.Admin.Gamification.ActionTypes;

public record CreateActionTypeRequest(string Code, string Name, string Description,
    int DefaultPoints, string Category, bool IsActive);

