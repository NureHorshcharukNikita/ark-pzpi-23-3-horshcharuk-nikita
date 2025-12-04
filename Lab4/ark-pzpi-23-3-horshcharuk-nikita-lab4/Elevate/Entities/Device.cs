namespace Elevate.Entities;

public class Device
{
    public int DeviceID { get; set; }
    public string Name { get; set; } = null!;
    public int TeamID { get; set; }
    public string DeviceKey { get; set; } = null!;
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastSeenAt { get; set; }

    public Team Team { get; set; } = null!;
    public ICollection<DeviceScan> DeviceScans { get; set; } = new List<DeviceScan>();
}

