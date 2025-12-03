namespace Elevate.Dtos.Teams;

public class TeamDetailDto : TeamDto
{
    public IReadOnlyCollection<TeamMemberDto> Members { get; set; }
        = Array.Empty<TeamMemberDto>();

    public IReadOnlyCollection<TeamLevelDto> Levels { get; set; }
        = Array.Empty<TeamLevelDto>();

    public IReadOnlyCollection<TeamBadgeDto> Badges { get; set; }
        = Array.Empty<TeamBadgeDto>();
}
