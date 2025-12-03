using Elevate.Dtos.Auth;
using Elevate.Entities;
using Elevate.Services.Auth.Tokens;

namespace Elevate.Mappings.Auth;

public static class AuthMappings
{
    public static LoginResponseDto ToLoginResponseDto(
        User user,
        JwtTokenResult tokenResult)
    {
        return new LoginResponseDto
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            User = new UserSummaryDto
            {
                Id = user.UserID,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        };
    }
}
