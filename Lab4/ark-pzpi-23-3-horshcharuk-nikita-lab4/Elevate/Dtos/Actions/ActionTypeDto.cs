namespace Elevate.Dtos.Actions;

public class ActionTypeDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DefaultPoints { get; set; }
    public string Category { get; set; } = null!;
    public bool IsActive { get; set; }
}
