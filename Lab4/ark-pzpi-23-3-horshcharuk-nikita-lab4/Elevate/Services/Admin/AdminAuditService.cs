using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Admin;

public class AdminAuditService : IAdminAuditService
{
    private readonly ElevateDbContext _dbContext;

    public AdminAuditService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<ActionEvent>> GetEventsAsync(
        int? teamId, int? userId, int? actionTypeId,
        DateTime? from, DateTime? to,
        CancellationToken ct = default)
    {
        var query = _dbContext.ActionEvents
            .Include(e => e.User)
            .Include(e => e.Team)
            .Include(e => e.ActionType)
            .AsQueryable();

        if (teamId.HasValue)
            query = query.Where(e => e.TeamID == teamId.Value);

        if (userId.HasValue)
            query = query.Where(e => e.UserID == userId.Value);

        if (actionTypeId.HasValue)
            query = query.Where(e => e.ActionTypeID == actionTypeId.Value);

        if (from.HasValue)
            query = query.Where(e => e.OccurredAt >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.OccurredAt <= to.Value);

        return await query
            .OrderByDescending(e => e.OccurredAt)
            .Take(500)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<DeviceScan>> GetDeviceScansAsync(
        int? teamId, int? userId,
        DateTime? from, DateTime? to,
        CancellationToken ct = default)
    {
        var query = _dbContext.DeviceScans
            .Include(s => s.Device)
            .Include(s => s.User)
            .AsQueryable();

        if (teamId.HasValue)
            query = query.Where(s => s.TeamID == teamId.Value);

        if (userId.HasValue)
            query = query.Where(s => s.UserID == userId.Value);

        if (from.HasValue)
            query = query.Where(s => s.ScannedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(s => s.ScannedAt <= to.Value);

        return await query
            .OrderByDescending(s => s.ScannedAt)
            .Take(500)
            .ToListAsync(ct);
    }
}

