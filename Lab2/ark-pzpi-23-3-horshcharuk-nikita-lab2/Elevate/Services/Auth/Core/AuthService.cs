using Elevate.Data;
using Elevate.Dtos.Auth;
using Elevate.Entities;
using Elevate.Mappings.Auth;
using Elevate.Services.Auth.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Auth.Core;

public class AuthService : IAuthService
{
    private readonly ElevateDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        ElevateDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var user = await FindUserAsync(request.LoginOrEmail, cancellationToken);

        ValidatePassword(user, request.Password);

        var tokenResult = _jwtTokenService.CreateToken(user);

        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return AuthMappings.ToLoginResponseDto(user, tokenResult);
    }

    public async Task<LoginResponseDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var normalizedLogin = Normalize(request.Login);
        var normalizedEmail = Normalize(request.Email);

        var exists = await _dbContext.Users.AnyAsync(
            u => u.Login.ToLower() == normalizedLogin ||
                 u.Email.ToLower() == normalizedEmail,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "A user with this login or email already exists.");
        }

        var user = new User
        {
            Login = request.Login.Trim(),
            Email = request.Email.Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var tokenResult = _jwtTokenService.CreateToken(user);

        return AuthMappings.ToLoginResponseDto(user, tokenResult);
    }

    private static string Normalize(string value) =>
        value.Trim().ToLowerInvariant();

    private async Task<User> FindUserAsync(
        string loginOrEmail,
        CancellationToken cancellationToken)
    {
        var normalized = Normalize(loginOrEmail);

        var user = await _dbContext.Users
            .Include(u => u.TeamMemberships)
            .FirstOrDefaultAsync(
                u => u.Login.ToLower() == normalized ||
                     u.Email.ToLower() == normalized,
                cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return user;
    }

    private void ValidatePassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
    }
}
