using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Admin;

public class AdminDeviceService : IAdminDeviceService
{
    private readonly ElevateDbContext _dbContext;

    public AdminDeviceService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Device> CreateDeviceAsync(string name, int teamId, string? location, CancellationToken ct = default)
    {
        var device = new Device
        {
            Name = name,
            TeamID = teamId,
            Location = location,
            DeviceKey = Guid.NewGuid().ToString("N"),
            IsActive = true,
            LastSeenAt = null
        };

        _dbContext.Devices.Add(device);
        await _dbContext.SaveChangesAsync(ct);

        return device;
    }

    public async Task SetDeviceActiveAsync(int deviceId, bool isActive, CancellationToken ct = default)
    {
        var device = await _dbContext.Devices.FirstOrDefaultAsync(d => d.DeviceID == deviceId, ct)
                      ?? throw new InvalidOperationException("Device not found");

        device.IsActive = isActive;
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Device>> GetAllDevicesAsync(CancellationToken ct = default)
    {
        return await _dbContext.Devices
            .OrderBy(d => d.DeviceID)
            .ToListAsync(ct);
    }
}

