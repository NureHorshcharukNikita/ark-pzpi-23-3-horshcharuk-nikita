using Elevate.Dtos.Teams;
using Elevate.Entities;

namespace Elevate.Mappings.Teams;

internal static class TeamMappings
{
    public static TeamMemberDto ToTeamMemberDto(TeamMember tm) => new()
    {
        UserId = tm.UserID,
        FullName = $"{tm.User.FirstName} {tm.User.LastName}",
        TeamRole = tm.TeamRole,
        TeamPoints = tm.TeamPoints,
        TeamLevel = tm.TeamLevel?.Name
    };

    public static TeamLevelDto ToTeamLevelDto(TeamLevel tl) => new()
    {
        Id = tl.TeamLevelID,
        Name = tl.Name,
        RequiredPoints = tl.RequiredPoints,
        OrderIndex = tl.OrderIndex
    };

    public static TeamBadgeDto ToTeamBadgeDto(TeamBadge tb) => new()
    {
        Id = tb.TeamBadgeID,
        Code = tb.Code,
        Name = tb.Name,
        Description = tb.Description,
        IconCode = tb.IconCode,
        ConditionType = tb.ConditionType,
        ConditionValue = tb.ConditionValue
    };

    public static LeaderboardEntryDto ToLeaderboardEntryDto(TeamMember tm, int rank) => new()
    {
        UserId = tm.UserID,
        FullName = $"{tm.User.FirstName} {tm.User.LastName}",
        TeamPoints = tm.TeamPoints,
        TeamLevel = tm.TeamLevel?.Name,
        Rank = rank
    };
}
