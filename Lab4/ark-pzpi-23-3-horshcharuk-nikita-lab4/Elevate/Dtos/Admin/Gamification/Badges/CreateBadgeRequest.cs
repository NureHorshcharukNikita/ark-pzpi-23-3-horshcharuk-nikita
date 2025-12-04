namespace Elevate.Dtos.Admin.Gamification.Badges;

public record CreateBadgeRequest(string Code, string Name, string Description,
    string IconCode, string ConditionType, int ConditionValue);

