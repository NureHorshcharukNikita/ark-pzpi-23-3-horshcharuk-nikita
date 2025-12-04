namespace Elevate.Services.Auth.Tokens;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "Elevate";
    public string Audience { get; set; } = "ElevateClients";
    public string SigningKey { get; set; } = null!;
    public int ExpiresMinutes { get; set; } = 60;
}