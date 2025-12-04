namespace Elevate.Dtos.Admin.Teams;

public record CreateTeamRequest(string Name, string? Description, int? ManagerUserId);

