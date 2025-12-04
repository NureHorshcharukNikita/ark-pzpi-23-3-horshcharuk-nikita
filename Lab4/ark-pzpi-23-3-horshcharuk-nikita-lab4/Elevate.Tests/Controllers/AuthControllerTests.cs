using Elevate.Data;
using Elevate.Dtos.Auth;
using Elevate.Entities;
using Elevate.Tests.TestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Elevate.Tests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var user = new User
        {
            Login = $"testuser_{uniqueId}",
            Email = $"test_{uniqueId}@test.com",
            FirstName = "Test",
            LastName = "User",
            Role = "User"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            LoginOrEmail = $"testuser_{uniqueId}",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrWhiteSpace();
        content.User.Should().NotBeNull();
        content.User.Login.Should().Be($"testuser_{uniqueId}");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        var user = new User
        {
            Login = "testuser",
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Role = "User"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            LoginOrEmail = "testuser",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
    {
        // Arrange
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            LoginOrEmail = "nonexistent",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmail_ReturnsToken()
    {
        // Arrange
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        var user = new User
        {
            Login = "testuser",
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Role = "User"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var request = new LoginRequestDto
        {
            LoginOrEmail = "test@test.com",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsToken_AndCreatesUser()
    {
        // Arrange
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        var request = new RegisterRequestDto
        {
            Login = "newuser",
            Email = "newuser@test.com",
            FirstName = "New",
            LastName = "User",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrWhiteSpace();
        content.User.Login.Should().Be("newuser");

        var userInDb = context.Users.SingleOrDefault(u => u.Login == "newuser");
        userInDb.Should().NotBeNull();
        userInDb!.Email.Should().Be("newuser@test.com");
    }

    [Fact]
    public async Task Register_WithExistingLoginOrEmail_ReturnsBadRequest()
    {
        // Arrange
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        var existingUser = new User
        {
            Login = "existing",
            Email = "existing@test.com",
            FirstName = "Exist",
            LastName = "User",
            Role = "User"
        };
        existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, "Password123!");
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var request = new RegisterRequestDto
        {
            Login = "existing",
            Email = "other@test.com",
            FirstName = "New",
            LastName = "User",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
