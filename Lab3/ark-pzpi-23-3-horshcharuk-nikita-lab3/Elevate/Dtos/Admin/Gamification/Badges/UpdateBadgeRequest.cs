namespace Elevate.Dtos.Admin.Gamification.Badges;

public record UpdateBadgeRequest(string Name, string Description,
    string IconCode, string ConditionType, int ConditionValue);

