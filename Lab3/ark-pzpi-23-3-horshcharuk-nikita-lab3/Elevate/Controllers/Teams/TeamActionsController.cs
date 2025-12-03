using Elevate.Dtos.Actions;
using Elevate.Services.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/teams/{teamId:int}/actions")]
[Authorize]
public class TeamActionsController : ControllerBase
{
    private readonly IActionEventService _service;

    public TeamActionsController(IActionEventService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ActionEventResultDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateActionEvent(
        int teamId,
        [FromBody] CreateActionEventRequest request,
        CancellationToken ct)
    {
        var dto = new CreateActionEventDto
        {
            UserId = request.UserId,
            TeamId = teamId,
            ActionTypeId = request.ActionTypeId,
            SourceType = request.SourceType,
            SourceUserId = request.SourceUserId,
            Comment = request.Comment,
            OccurredAt = request.OccurredAt
        };

        var result = await _service.CreateActionEventAsync(dto, null, ct);
        
        var resultDto = new ActionEventResultDto
        {
            ActionEventId = result.Id,
            UserId = result.UserId,
            TeamId = result.TeamId,
            PointsAwarded = result.PointsAwarded,
            TotalTeamPoints = result.TeamPoints,
            NewTeamLevelName = result.TeamLevel,
            NewBadges = result.NewTeamBadges.Select(b => b.BadgeName).ToList()
        };

        return CreatedAtAction(nameof(CreateActionEvent), new { id = result.Id }, resultDto);
    }
}

