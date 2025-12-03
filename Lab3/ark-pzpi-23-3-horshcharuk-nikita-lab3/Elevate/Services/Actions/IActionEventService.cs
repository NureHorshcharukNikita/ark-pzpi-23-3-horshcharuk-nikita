using Elevate.Dtos.Actions;
using Elevate.Entities;

namespace Elevate.Services.Actions;

public interface IActionEventService
{
    Task<ActionEventDto> CreateActionEventAsync(
        CreateActionEventDto dto,
        int? authenticatedUserId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ActionEventDto>> GetActionEventsAsync(
        int? userId,
        int? teamId,
        CancellationToken cancellationToken);

    Task<ActionEvent?> GetActionEventByIdAsync(
        int id,
        CancellationToken cancellationToken);
}
