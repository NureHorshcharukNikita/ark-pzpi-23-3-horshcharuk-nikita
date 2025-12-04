using Elevate.Entities;

namespace Elevate.Services.Admin;

public interface IAdminDeviceService
{
    Task<Device> CreateDeviceAsync(string name, int teamId, string? location, CancellationToken ct = default);
    Task SetDeviceActiveAsync(int deviceId, bool isActive, CancellationToken ct = default);
    Task<IReadOnlyList<Device>> GetAllDevicesAsync(CancellationToken ct = default);
}

