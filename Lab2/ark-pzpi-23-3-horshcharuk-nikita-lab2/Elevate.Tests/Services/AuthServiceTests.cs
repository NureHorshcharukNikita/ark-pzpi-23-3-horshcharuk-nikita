using Elevate.Data;
using Elevate.Dtos.Auth;
using Elevate.Entities;
using Elevate.Services.Auth;
using Elevate.Services.Auth.Core;
using Elevate.Services.Auth.Tokens;
using Elevate.Tests.TestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Elevate.Tests.Services;

public class AuthServiceTests
{
    private static IOptions<JwtOptions> CreateJwtOptions() =>
        Options.Create(new JwtOptions
        {
            Issuer = "Elevate.Tests",
            Audience = "Elevate.Tests",
            SigningKey = "UnitTestSigningKey_ShouldBeLongEnough_123456",
            ExpiresMinutes = 30
        });

    [Fact]
    public async Task LoginAsync_ReturnsToken_ForValidCredentials()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var passwordHasher = new PasswordHasher<User>();

        var (service, user) = await CreateAuthServiceWithUserAsync(
            context,
            passwordHasher,
            "Password123!");

        // Act
        var result = await service.LoginAsync(new LoginRequestDto
        {
            LoginOrEmail = "tester",
            Password = "Password123!"
        }, CancellationToken.None);

        // Assert
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.User.Id.Should().Be(user.UserID);
        result.User.Login.Should().Be("tester");
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorized_ForInvalidPassword()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var passwordHasher = new PasswordHasher<User>();

        var (service, _) = await CreateAuthServiceWithUserAsync(
            context,
            passwordHasher,
            "Password123!");

        var act = () => service.LoginAsync(new LoginRequestDto
        {
            LoginOrEmail = "tester",
            Password = "WrongPass!"
        }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task RegisterAsync_CreatesUser_AndReturnsToken()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var passwordHasher = new PasswordHasher<User>();

        var service = new AuthService(
            context,
            new JwtTokenService(CreateJwtOptions()),
            passwordHasher);

        var request = new RegisterRequestDto
        {
            Login = "newuser",
            Email = "newuser@test.com",
            FirstName = "New",
            LastName = "User",
            Password = "Password123!"
        };

        // Act
        var result = await service.RegisterAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.User.Login.Should().Be("newuser");

        var userInDb = context.Users.SingleOrDefault(u => u.Login == "newuser");
        userInDb.Should().NotBeNull();
        userInDb!.Email.Should().Be("newuser@test.com");
        userInDb.PasswordHash.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RegisterAsync_ThrowsInvalidOperation_WhenLoginOrEmailAlreadyExists()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var passwordHasher = new PasswordHasher<User>();

        // существующий пользователь
        var existingUser = new User
        {
            Login = "tester",
            Email = "tester@elevate",
            FirstName = "Test",
            LastName = "User",
            Role = "User"
        };
        existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, "Password123!");
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var service = new AuthService(
            context,
            new JwtTokenService(CreateJwtOptions()),
            passwordHasher);

        var request = new RegisterRequestDto
        {
            Login = "tester",
            Email = "other@test.com",
            FirstName = "New",
            LastName = "User",
            Password = "Password123!"
        };

        // Act
        var act = () => service.RegisterAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists.*");
    }

    private static async Task<(AuthService service, User user)> CreateAuthServiceWithUserAsync(
        ElevateDbContext context,
        IPasswordHasher<User> passwordHasher,
        string rawPassword)
    {
        var user = new User
        {
            Login = "tester",
            Email = "tester@elevate",
            FirstName = "Test",
            LastName = "User",
            Role = "User"
        };

        user.PasswordHash = passwordHasher.HashPassword(user, rawPassword);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthService(
            context,
            new JwtTokenService(CreateJwtOptions()),
            passwordHasher);

        return (service, user);
    }
}
