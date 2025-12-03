using Elevate.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminAuditController : ControllerBase
{
    private readonly IAdminAuditService _service;

    public AdminAuditController(IAdminAuditService service)
    {
        _service = service;
    }

    [HttpGet("events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvents(
        [FromQuery] int? teamId,
        [FromQuery] int? userId,
        [FromQuery] int? actionTypeId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var events = await _service.GetEventsAsync(teamId, userId, actionTypeId, from, to, ct);

        var result = events.Select(e => new
        {
            e.ActionEventID,
            e.TeamID,
            TeamName = e.Team.Name,
            e.UserID,
            UserName = e.User.FirstName + " " + e.User.LastName,
            e.ActionTypeID,
            ActionTypeCode = e.ActionType.Code,
            e.PointsAwarded,
            e.SourceType,
            e.SourceUserID,
            e.Comment,
            e.OccurredAt,
            e.CreatedAt,
            e.IsValid
        });

        return Ok(result);
    }

    [HttpGet("device-scans")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeviceScans(
        [FromQuery] int? teamId,
        [FromQuery] int? userId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var scans = await _service.GetDeviceScansAsync(teamId, userId, from, to, ct);

        var result = scans.Select(s => new
        {
            s.DeviceScanID,
            s.TeamID,
            s.UserID,
            UserName = s.User.FirstName + " " + s.User.LastName,
            s.DeviceID,
            DeviceName = s.Device.Name,
            s.ScannedAt
        });

        return Ok(result);
    }
}

