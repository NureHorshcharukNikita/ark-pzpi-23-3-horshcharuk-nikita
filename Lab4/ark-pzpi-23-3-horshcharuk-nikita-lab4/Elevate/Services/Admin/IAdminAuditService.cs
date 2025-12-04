using Elevate.Entities;

namespace Elevate.Services.Admin;

public interface IAdminAuditService
{
    Task<IReadOnlyList<ActionEvent>> GetEventsAsync(
        int? teamId, int? userId, int? actionTypeId,
        DateTime? from, DateTime? to,
        CancellationToken ct = default);

    Task<IReadOnlyList<DeviceScan>> GetDeviceScansAsync(
        int? teamId, int? userId,
        DateTime? from, DateTime? to,
        CancellationToken ct = default);
}

