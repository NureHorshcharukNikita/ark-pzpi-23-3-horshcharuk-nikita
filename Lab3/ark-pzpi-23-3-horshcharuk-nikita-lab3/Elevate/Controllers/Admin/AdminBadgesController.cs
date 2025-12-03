using Elevate.Dtos.Admin.Gamification.Badges;
using Elevate.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/admin/badges")]
[Authorize(Roles = "Admin")]
public class AdminBadgesController : ControllerBase
{
    private readonly ITeamBadgesAdminService _service;

    public AdminBadgesController(ITeamBadgesAdminService service)
    {
        _service = service;
    }

    [HttpGet("teams/{teamId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBadges(int teamId, CancellationToken ct)
    {
        var badges = await _service.GetTeamBadgesAsync(teamId, ct);
        return Ok(badges);
    }

    [HttpPost("teams/{teamId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBadge(int teamId, [FromBody] CreateBadgeRequest request, CancellationToken ct)
    {
        var badge = await _service.CreateTeamBadgeAsync(teamId, request.Code, request.Name, request.Description,
            request.IconCode, request.ConditionType, request.ConditionValue, ct);
        return Ok(badge);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateBadge(int id, [FromBody] UpdateBadgeRequest request, CancellationToken ct)
    {
        await _service.UpdateTeamBadgeAsync(id, request.Name, request.Description, request.IconCode,
            request.ConditionType, request.ConditionValue, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteBadge(int id, CancellationToken ct)
    {
        await _service.DeleteTeamBadgeAsync(id, ct);
        return NoContent();
    }
}

