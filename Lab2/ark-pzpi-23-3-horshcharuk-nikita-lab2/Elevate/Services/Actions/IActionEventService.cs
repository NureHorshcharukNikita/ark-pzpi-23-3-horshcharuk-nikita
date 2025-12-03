using Elevate.Dtos.Actions;
using Elevate.Entities;

namespace Elevate.Services.Actions;

public interface IActionEventService
{
    Task<ActionEventDto> CreateAsync(
        CreateActionEventDto dto,
        int? authenticatedUserId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ActionEventDto>> GetAsync(
        int? userId,
        int? teamId,
        CancellationToken cancellationToken);

    Task<ActionEvent?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);
}
