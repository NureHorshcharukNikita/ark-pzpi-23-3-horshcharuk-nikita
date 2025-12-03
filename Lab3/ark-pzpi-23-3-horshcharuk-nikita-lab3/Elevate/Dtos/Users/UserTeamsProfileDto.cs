namespace Elevate.Dtos.Users;

public class UserTeamsProfileDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public IReadOnlyList<UserTeamInfoDto> Teams { get; set; } = Array.Empty<UserTeamInfoDto>();
}

