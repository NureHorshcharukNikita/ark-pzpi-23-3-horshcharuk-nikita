using Elevate.Entities;

namespace Elevate.Services.Auth.Tokens;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(User user);
}