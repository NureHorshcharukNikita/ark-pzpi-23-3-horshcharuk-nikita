using Elevate.Dtos.Admin.Devices;
using Elevate.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/admin/devices")]
[Authorize(Roles = "Admin")]
public class AdminDevicesController : ControllerBase
{
    private readonly IAdminDeviceService _service;

    public AdminDevicesController(IAdminDeviceService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDevices(CancellationToken ct)
    {
        var devices = await _service.GetAllDevicesAsync(ct);
        return Ok(devices.Select(d => new
        {
            d.DeviceID,
            d.Name,
            d.TeamID,
            d.Location,
            d.DeviceKey,
            d.IsActive,
            d.LastSeenAt
        }));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceRequest request, CancellationToken ct)
    {
        var device = await _service.CreateDeviceAsync(request.Name, request.TeamId, request.Location, ct);
        return Ok(new { device.DeviceID, device.DeviceKey });
    }

    [HttpPost("{id:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateDevice(int id, CancellationToken ct)
    {
        await _service.SetDeviceActiveAsync(id, true, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeactivateDevice(int id, CancellationToken ct)
    {
        await _service.SetDeviceActiveAsync(id, false, ct);
        return NoContent();
    }
}

