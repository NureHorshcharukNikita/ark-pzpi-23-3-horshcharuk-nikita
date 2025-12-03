using Elevate.Dtos.Admin.Gamification.Levels;
using Elevate.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/admin/levels")]
[Authorize(Roles = "Admin")]
public class AdminLevelsController : ControllerBase
{
    private readonly ITeamLevelsAdminService _service;

    public AdminLevelsController(ITeamLevelsAdminService service)
    {
        _service = service;
    }

    [HttpGet("teams/{teamId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLevels(int teamId, CancellationToken ct)
    {
        var levels = await _service.GetTeamLevelsAsync(teamId, ct);
        return Ok(levels);
    }

    [HttpPost("teams/{teamId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateLevel(int teamId, [FromBody] CreateLevelRequest request, CancellationToken ct)
    {
        var level = await _service.CreateTeamLevelAsync(teamId, request.Name, request.RequiredPoints, request.OrderIndex, ct);
        return Ok(level);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateLevel(int id, [FromBody] UpdateLevelRequest request, CancellationToken ct)
    {
        await _service.UpdateTeamLevelAsync(id, request.Name, request.RequiredPoints, request.OrderIndex, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteLevel(int id, CancellationToken ct)
    {
        await _service.DeleteTeamLevelAsync(id, ct);
        return NoContent();
    }
}

