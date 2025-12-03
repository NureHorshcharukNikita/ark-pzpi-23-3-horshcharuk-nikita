namespace Elevate.Services.Auth.Tokens;

public sealed record JwtTokenResult(
    string Token,
    DateTime ExpiresAt
 );
