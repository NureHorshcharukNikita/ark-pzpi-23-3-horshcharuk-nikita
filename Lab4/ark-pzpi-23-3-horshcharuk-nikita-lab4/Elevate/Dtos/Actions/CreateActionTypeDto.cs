namespace Elevate.Dtos.Actions;

public class CreateActionTypeDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DefaultPoints { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;
}
