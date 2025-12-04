using Elevate.Dtos.Analytics;
using Elevate.Services.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Authorize(Roles = "Manager,Admin")]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("overview")]
    [ProducesResponseType(typeof(TeamAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Overview([FromQuery] int teamId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var analytics = await _analyticsService.GetOverviewAsync(teamId, from, to, cancellationToken);
        return analytics == null ? NotFound() : Ok(analytics);
    }
}

