namespace Elevate.Dtos.Admin.Gamification.ActionTypes;

public record UpdateActionTypeRequest(string Name, string Description,
    int DefaultPoints, string Category, bool IsActive);

