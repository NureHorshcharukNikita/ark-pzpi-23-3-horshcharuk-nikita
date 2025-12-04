using Elevate.Dtos.Users;

namespace Elevate.Services.Users;

public interface IUserProfileService
{
    Task<UserTeamsProfileDto> GetUserTeamsProfileAsync(int userId, CancellationToken ct = default);
}

