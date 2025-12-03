using Elevate.Data;
using Elevate.Dtos.Actions;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Actions;

public class ActionEventValidator
{
    private readonly ElevateDbContext _dbContext;

    public ActionEventValidator(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int ResolveUserId(CreateActionEventDto dto, int? authenticatedUserId)
    {
        return dto.UserId
               ?? authenticatedUserId
               ?? throw new InvalidOperationException("Missing user context");
    }

    public async Task<ActionType> ValidateAndGetActionTypeAsync(
        CreateActionEventDto dto,
        CancellationToken cancellationToken)
    {
        var actionType = await _dbContext.ActionTypes
            .FirstOrDefaultAsync(
                at => at.ActionTypeID == dto.ActionTypeId && at.TeamID == dto.TeamId,
                cancellationToken);

        if (actionType == null || !actionType.IsActive)
        {
            throw new InvalidOperationException("Invalid action type for this team");
        }

        return actionType;
    }

    public async Task<TeamMember> ValidateAndGetMembershipAsync(
        int userId,
        int teamId,
        CancellationToken cancellationToken)
    {
        var membership = await _dbContext.TeamMembers
            .Include(tm => tm.TeamLevel)
            .FirstOrDefaultAsync(
                tm => tm.UserID == userId && tm.TeamID == teamId,
                cancellationToken);

        if (membership == null)
        {
            throw new InvalidOperationException("User is not a member of this team");
        }

        return membership;
    }
}
