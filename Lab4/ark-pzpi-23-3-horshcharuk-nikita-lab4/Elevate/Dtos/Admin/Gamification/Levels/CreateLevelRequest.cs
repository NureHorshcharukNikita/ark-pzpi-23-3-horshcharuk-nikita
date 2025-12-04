namespace Elevate.Dtos.Admin.Gamification.Levels;

public record CreateLevelRequest(string Name, int RequiredPoints, int OrderIndex);

