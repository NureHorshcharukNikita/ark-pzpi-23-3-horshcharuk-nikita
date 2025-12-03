namespace Elevate.Dtos.Admin.Devices;

public record CreateDeviceRequest(string Name, int TeamId, string? Location);

