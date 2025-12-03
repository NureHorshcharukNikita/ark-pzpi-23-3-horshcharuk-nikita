namespace Elevate.Entities;

public class DeviceScan
{
    public int DeviceScanID { get; set; }
    public int DeviceID { get; set; }
    public int TeamID { get; set; }
    public int UserID { get; set; }
    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

    public Device Device { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
}

