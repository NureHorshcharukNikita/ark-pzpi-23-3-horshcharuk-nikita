using Elevate.Dtos.Actions;
using Elevate.Dtos.Teams;
using Elevate.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Authorize(Roles = "Manager,Admin")]
[Route("api/teams/{teamId:int}/gamification")]
public class TeamGamificationController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamGamificationController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost("levels")]
    [ProducesResponseType(typeof(TeamLevelDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateLevel(int teamId, [FromBody] CreateTeamLevelDto dto, CancellationToken cancellationToken)
    {
        var level = await _teamService.CreateLevelAsync(teamId, dto, cancellationToken);
        return CreatedAtAction(nameof(CreateLevel), new { teamId, levelId = level.Id }, level);
    }

    [HttpPost("badges")]
    [ProducesResponseType(typeof(TeamBadgeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBadge(int teamId, [FromBody] CreateTeamBadgeDto dto, CancellationToken cancellationToken)
    {
        var badge = await _teamService.CreateBadgeAsync(teamId, dto, cancellationToken);
        return CreatedAtAction(nameof(CreateBadge), new { teamId, badgeId = badge.Id }, badge);
    }

    [HttpPost("action-types")]
    [ProducesResponseType(typeof(ActionTypeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateActionType(int teamId, [FromBody] CreateActionTypeDto dto, CancellationToken cancellationToken)
    {
        var actionType = await _teamService.CreateActionTypeAsync(teamId, dto, cancellationToken);

        return CreatedAtAction(
            nameof(CreateActionType),
            new { teamId, actionTypeId = actionType.Id },
            actionType
        );
    }
}
