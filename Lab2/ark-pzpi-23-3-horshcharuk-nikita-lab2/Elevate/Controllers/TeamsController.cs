using Elevate.Dtos.Teams;
using Elevate.Services.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Authorize]
[Route("api/teams")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TeamDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeams(CancellationToken cancellationToken)
    {
        var teams = await _teamService.GetTeamsAsync(cancellationToken);
        return Ok(teams);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TeamDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeam(int id, CancellationToken cancellationToken)
    {
        var team = await _teamService.GetTeamAsync(id, cancellationToken);
        return team == null ? NotFound() : Ok(team);
    }

    [HttpGet("{id:int}/members")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TeamMemberDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMembers(int id, CancellationToken cancellationToken)
    {
        var members = await _teamService.GetMembersAsync(id, cancellationToken);
        return Ok(members);
    }

    [HttpGet("{id:int}/leaderboard")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LeaderboardEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaderboard(int id, CancellationToken cancellationToken)
    {
        var leaderboard = await _teamService.GetLeaderboardAsync(id, cancellationToken);
        return Ok(leaderboard);
    }
}

