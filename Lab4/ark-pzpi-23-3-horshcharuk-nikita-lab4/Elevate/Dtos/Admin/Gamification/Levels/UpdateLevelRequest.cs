namespace Elevate.Dtos.Admin.Gamification.Levels;

public record UpdateLevelRequest(string Name, int RequiredPoints, int OrderIndex);

