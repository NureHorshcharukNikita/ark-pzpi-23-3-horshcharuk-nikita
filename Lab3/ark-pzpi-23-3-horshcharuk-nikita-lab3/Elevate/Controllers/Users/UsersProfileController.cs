using Elevate.Dtos.Users;
using Elevate.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/users/{userId:int}/teams-profile")]
[Authorize]
public class UsersProfileController : ControllerBase
{
    private readonly IUserProfileService _service;

    public UsersProfileController(IUserProfileService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserTeamsProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserTeamsProfile(int userId, CancellationToken ct)
    {
        var profile = await _service.GetUserTeamsProfileAsync(userId, ct);
        return Ok(profile);
    }
}

