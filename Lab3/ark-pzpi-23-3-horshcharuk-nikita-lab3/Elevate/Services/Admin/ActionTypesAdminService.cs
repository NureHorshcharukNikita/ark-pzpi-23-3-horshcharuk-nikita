using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Admin;

public class ActionTypesAdminService : IActionTypesAdminService
{
    private readonly ElevateDbContext _dbContext;

    public ActionTypesAdminService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<ActionType>> GetActionTypesAsync(int teamId, CancellationToken ct = default)
    {
        return await _dbContext.ActionTypes
            .Where(a => a.TeamID == teamId)
            .OrderBy(a => a.Code)
            .ToListAsync(ct);
    }

    public async Task<ActionType> CreateActionTypeAsync(int teamId, string code, string name, string description,
        int defaultPoints, string category, bool isActive, CancellationToken ct = default)
    {
        var actionType = new ActionType
        {
            TeamID = teamId,
            Code = code,
            Name = name,
            Description = description,
            DefaultPoints = defaultPoints,
            Category = category,
            IsActive = isActive
        };

        _dbContext.ActionTypes.Add(actionType);
        await _dbContext.SaveChangesAsync(ct);
        return actionType;
    }

    public async Task UpdateActionTypeAsync(int actionTypeId, string name, string description,
        int defaultPoints, string category, bool isActive, CancellationToken ct = default)
    {
        var actionType = await _dbContext.ActionTypes.FirstOrDefaultAsync(a => a.ActionTypeID == actionTypeId, ct)
                         ?? throw new InvalidOperationException("ActionType not found");

        actionType.Name = name;
        actionType.Description = description;
        actionType.DefaultPoints = defaultPoints;
        actionType.Category = category;
        actionType.IsActive = isActive;

        await _dbContext.SaveChangesAsync(ct);
    }
}

