using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Elevate.Entities;

namespace Elevate.Services.Auth.Tokens;

public static class JwtClaimFactory
{
    private const string DefaultRole = "User";

    public static IEnumerable<Claim> CreateClaims(User user)
    {
        var userId = user.UserID.ToString();
        var role = string.IsNullOrWhiteSpace(user.Role) ? DefaultRole : user.Role;

        return
        [
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Login),
            new Claim(ClaimTypes.Role, role)
        ];
    }
}
