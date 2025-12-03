using Elevate.Dtos.Auth;
using Elevate.Entities;
using Elevate.Mappings.Auth;
using Elevate.Services.Auth.Tokens;
using Microsoft.AspNetCore.Identity;

namespace Elevate.Services.Auth.Core;

public class AuthService : IAuthService
{
    private readonly UserRepository _userRepository;
    private readonly UserValidator _userValidator;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        UserRepository userRepository,
        UserValidator userValidator,
        IJwtTokenService jwtTokenService,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByLoginOrEmailAsync(request.LoginOrEmail, cancellationToken);
        _userValidator.ValidatePassword(user, request.Password);

        var tokenResult = _jwtTokenService.CreateToken(user);
        await _userRepository.UpdateLastLoginAsync(user, cancellationToken);

        return AuthMappings.ToLoginResponseDto(user, tokenResult);
    }

    public async Task<LoginResponseDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var exists = await _userRepository.ExistsByLoginOrEmailAsync(
            request.Login,
            request.Email,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "A user with this login or email already exists.");
        }

        var user = CreateUserEntity(request);
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddUserAsync(user, cancellationToken);

        var tokenResult = _jwtTokenService.CreateToken(user);
        return AuthMappings.ToLoginResponseDto(user, tokenResult);
    }

    private static User CreateUserEntity(RegisterRequestDto request)
    {
        return new User
        {
            Login = request.Login.Trim(),
            Email = request.Email.Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
