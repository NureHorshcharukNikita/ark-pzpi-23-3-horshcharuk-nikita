using Elevate.Dtos.IoT;
using Elevate.Dtos.Teams;
using Elevate.Entities;

namespace Elevate.Mappings.IoT;

public static class IoTMappings
{
    public static DeviceScanResponseDto ToDeviceScanResponseDto(
        TeamMember membership,
        IReadOnlyCollection<string> recentBadges)
    {
        return new DeviceScanResponseDto
        {
            FullName = $"{membership.User.FirstName} {membership.User.LastName}",
            TeamId = membership.TeamID,
            TeamName = membership.Team.Name,
            TeamPoints = membership.TeamPoints,
            TeamLevel = membership.TeamLevel?.Name,
            RecentBadges = recentBadges
        };
    }

    public static LeaderboardEntryDto ToLeaderboardEntryDto(
        TeamMember member,
        int rank)
    {
        return new LeaderboardEntryDto
        {
            UserId = member.UserID,
            FullName = $"{member.User.FirstName} {member.User.LastName}",
            TeamPoints = member.TeamPoints,
            TeamLevel = member.TeamLevel?.Name,
            Rank = rank
        };
    }
}
