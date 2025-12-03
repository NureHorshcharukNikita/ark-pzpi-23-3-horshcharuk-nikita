using Elevate.Dtos.IoT;
using Elevate.Services.IoT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/iot")]
public class IoTController : ControllerBase
{
    private readonly IIoTService _iotService;

    public IoTController(IIoTService iotService)
    {
        _iotService = iotService;
    }

    [HttpPost("scan")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DeviceScanResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Scan([FromBody] DeviceScanRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _iotService.ProcessScanAsync(dto, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("leaderboard")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyCollection<Dtos.Teams.LeaderboardEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Leaderboard([FromQuery] int teamId, CancellationToken cancellationToken)
    {
        var leaderboard = await _iotService.GetLeaderboardAsync(teamId, cancellationToken);
        return Ok(leaderboard);
    }
}

