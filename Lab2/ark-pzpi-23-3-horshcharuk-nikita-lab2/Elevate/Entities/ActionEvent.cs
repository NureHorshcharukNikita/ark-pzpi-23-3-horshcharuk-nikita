namespace Elevate.Entities;

public class ActionEvent
{
    public int ActionEventID { get; set; }
    public int UserID { get; set; }
    public int TeamID { get; set; }
    public int ActionTypeID { get; set; }
    public string SourceType { get; set; } = null!;
    public int? SourceUserID { get; set; }
    public string? Comment { get; set; }
    public int PointsAwarded { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsValid { get; set; } = true;

    public User User { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public ActionType ActionType { get; set; } = null!;
    public User? SourceUser { get; set; }
}

