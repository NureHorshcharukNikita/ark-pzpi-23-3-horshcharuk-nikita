using Elevate.Dtos.Users;
using Elevate.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var profile = await _userService.GetProfileAsync(User, cancellationToken);
        if (profile == null)
        {
            return NotFound();
        }

        return Ok(profile);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var profile = await _userService.GetProfileAsync(id, cancellationToken);
        if (profile == null)
        {
            return NotFound();
        }

        return Ok(profile);
    }
}

