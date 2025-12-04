using Elevate.Dtos.Auth;

namespace Elevate.Services.Auth.Core;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);

    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
}
