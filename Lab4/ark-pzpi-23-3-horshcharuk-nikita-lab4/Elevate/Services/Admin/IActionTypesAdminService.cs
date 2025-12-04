using Elevate.Entities;

namespace Elevate.Services.Admin;

public interface IActionTypesAdminService
{
    Task<IReadOnlyList<ActionType>> GetActionTypesAsync(int teamId, CancellationToken ct = default);
    Task<ActionType> CreateActionTypeAsync(int teamId, string code, string name, string description,
        int defaultPoints, string category, bool isActive, CancellationToken ct = default);
    Task UpdateActionTypeAsync(int actionTypeId, string name, string description,
        int defaultPoints, string category, bool isActive, CancellationToken ct = default);
}

