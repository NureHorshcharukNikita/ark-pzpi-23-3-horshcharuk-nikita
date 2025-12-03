using Elevate.Dtos.Admin.Users;
using Elevate.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elevate.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _service;

    public AdminUsersController(IAdminUserService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _service.GetAllAsync(ct);

        var result = users.Select(u => new
        {
            u.UserID,
            u.Login,
            u.Email,
            u.Role,
            u.IsActive,
            u.CreatedAt,
            u.LastLoginAt
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var user = await _service.GetByIdAsync(id, ct);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.UserID,
            user.Login,
            user.Email,
            user.Role,
            user.IsActive,
            user.FirstName,
            user.LastName,
            user.CreatedAt,
            user.LastLoginAt
        });
    }

    [HttpPost("{id:int}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetRole(int id, [FromBody] SetRoleRequest request, CancellationToken ct)
    {
        await _service.SetRoleAsync(id, request.Role, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> BlockUser(int id, CancellationToken ct)
    {
        await _service.SetActiveAsync(id, false, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/unblock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnblockUser(int id, CancellationToken ct)
    {
        await _service.SetActiveAsync(id, true, ct);
        return NoContent();
    }
}

