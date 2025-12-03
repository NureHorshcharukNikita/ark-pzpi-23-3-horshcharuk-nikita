using System.Security.Claims;
using Elevate.Dtos.Users;

namespace Elevate.Services.Users;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(int userId, CancellationToken cancellationToken);
    Task<UserProfileDto?> GetProfileAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
}
