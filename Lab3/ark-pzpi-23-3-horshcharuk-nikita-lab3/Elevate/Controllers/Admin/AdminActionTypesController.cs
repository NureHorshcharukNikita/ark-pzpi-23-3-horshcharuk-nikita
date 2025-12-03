using Elevate.Dtos.Admin.Gamification.ActionTypes;
using Elevate.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/admin/action-types")]
[Authorize(Roles = "Admin")]
public class AdminActionTypesController : ControllerBase
{
    private readonly IActionTypesAdminService _service;

    public AdminActionTypesController(IActionTypesAdminService service)
    {
        _service = service;
    }

    [HttpGet("teams/{teamId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActionTypes(int teamId, CancellationToken ct)
    {
        var actions = await _service.GetActionTypesAsync(teamId, ct);
        return Ok(actions);
    }

    [HttpPost("teams/{teamId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateActionType(int teamId, [FromBody] CreateActionTypeRequest request, CancellationToken ct)
    {
        var actionType = await _service.CreateActionTypeAsync(teamId, request.Code, request.Name, request.Description,
            request.DefaultPoints, request.Category, request.IsActive, ct);
        return Ok(actionType);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateActionType(int id, [FromBody] UpdateActionTypeRequest request, CancellationToken ct)
    {
        await _service.UpdateActionTypeAsync(id, request.Name, request.Description,
            request.DefaultPoints, request.Category, request.IsActive, ct);
        return NoContent();
    }
}

