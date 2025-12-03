namespace Elevate.Entities;

public class ActionType
{
    public int ActionTypeID { get; set; }
    public int TeamID { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DefaultPoints { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;

    public Team Team { get; set; } = null!;
    public ICollection<ActionEvent> ActionEvents { get; set; } = new List<ActionEvent>();
}

