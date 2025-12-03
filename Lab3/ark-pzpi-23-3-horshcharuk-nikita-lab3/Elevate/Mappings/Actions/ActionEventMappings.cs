using System.Linq.Expressions;
using Elevate.Dtos.Actions;
using Elevate.Entities;

namespace Elevate.Mappings.Actions;

public static class ActionEventMappings
{
    public static readonly Expression<Func<ActionEvent, ActionEventDto>> ToListItem =
        ae => new ActionEventDto
        {
            Id = ae.ActionEventID,
            UserId = ae.UserID,
            TeamId = ae.TeamID,
            ActionTypeId = ae.ActionTypeID,
            SourceType = ae.SourceType,
            PointsAwarded = ae.PointsAwarded,
            OccurredAt = ae.OccurredAt,
            CreatedAt = ae.CreatedAt,
            TeamPoints = ae.User.TeamMemberships
                .Where(tm => tm.TeamID == ae.TeamID)
                .Select(tm => tm.TeamPoints)
                .FirstOrDefault(),
            TeamLevel = ae.User.TeamMemberships
                .Where(tm => tm.TeamID == ae.TeamID)
                .Select(tm => tm.TeamLevel != null ? tm.TeamLevel.Name : null)
                .FirstOrDefault(),
            NewTeamBadges = Array.Empty<EarnedTeamBadgeDto>()
        };

    public static ActionEventDto ToActionEventDto(
        ActionEvent actionEvent,
        TeamMember membership,
        string? membershipLevelName,
        IReadOnlyCollection<UserTeamBadge> earnedBadges,
        IReadOnlyDictionary<int, string> badgeLookup)
    {
        return new ActionEventDto
        {
            Id = actionEvent.ActionEventID,
            UserId = actionEvent.UserID,
            TeamId = actionEvent.TeamID,
            ActionTypeId = actionEvent.ActionTypeID,
            SourceType = actionEvent.SourceType,
            PointsAwarded = actionEvent.PointsAwarded,
            OccurredAt = actionEvent.OccurredAt,
            CreatedAt = actionEvent.CreatedAt,
            TeamPoints = membership.TeamPoints,
            TeamLevel = membershipLevelName,
            NewTeamBadges = earnedBadges
                .Select(b => new EarnedTeamBadgeDto
                {
                    TeamBadgeId = b.TeamBadgeID,
                    BadgeName = badgeLookup.TryGetValue(b.TeamBadgeID, out var name)
                        ? name
                        : "Team Badge"
                })
                .ToArray()
        };
    }
}
