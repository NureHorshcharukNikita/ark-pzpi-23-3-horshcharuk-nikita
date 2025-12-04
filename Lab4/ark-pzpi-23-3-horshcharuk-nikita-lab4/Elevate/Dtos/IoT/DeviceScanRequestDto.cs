namespace Elevate.Dtos.IoT;

public class DeviceScanRequestDto
{
    public string DeviceKey { get; set; } = null!;
    public int UserId { get; set; }
    public string? ActionCode { get; set; }
}
