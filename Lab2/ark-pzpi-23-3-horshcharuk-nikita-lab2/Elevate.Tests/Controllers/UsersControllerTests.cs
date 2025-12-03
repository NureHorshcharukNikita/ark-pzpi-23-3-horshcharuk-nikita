using Elevate.Dtos.Users;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Elevate.Tests.Controllers;

public class UsersControllerTests : ControllerTestBase
{
    public UsersControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Me_WithAuthenticatedUser_ReturnsOk()
    {
        // Arrange
        var (user, client) = await CreateAuthenticatedUserAndClientAsync();

        // Act
        var response = await client.GetAsync("/api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.UserID);
        result.Login.Should().Be(user.Login);
    }

    [Fact]
    public async Task Me_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync("/api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithManagerRole_ReturnsOk()
    {
        // Arrange
        var (user, client) = await CreateAuthenticatedUserAndClientAsync(role: "Manager");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var targetUser = new User
        {
            Login = "targetuser",
            Email = "target@test.com",
            FirstName = "Target",
            LastName = "User",
            PasswordHash = "hash",
            Role = "User"
        };
        context.Users.Add(targetUser);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/users/{targetUser.UserID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(targetUser.UserID);
        result.Login.Should().Be("targetuser");
    }

    [Fact]
    public async Task GetById_WithAdminRole_ReturnsOk()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync(role: "Admin");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var targetUser = new User
        {
            Login = "targetuser",
            Email = "target@test.com",
            FirstName = "Target",
            LastName = "User",
            PasswordHash = "hash",
            Role = "User"
        };
        context.Users.Add(targetUser);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/users/{targetUser.UserID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WithoutManagerRole_ReturnsForbidden()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync(role: "User");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var targetUser = new User
        {
            Login = "targetuser",
            Email = "target@test.com",
            FirstName = "Target",
            LastName = "User",
            PasswordHash = "hash",
            Role = "User"
        };
        context.Users.Add(targetUser);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/users/{targetUser.UserID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync(role: "Manager");

        // Act
        var response = await client.GetAsync("/api/users/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync("/api/users/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithUserInTeams_ReturnsProfileWithTeams()
    {
        // Arrange
        var (user, client) = await CreateAuthenticatedUserAndClientAsync();
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        var existingUser = await context.Users.FindAsync(user.UserID);
        var membership = new TeamMember
        {
            Team = team,
            User = existingUser!,
            TeamPoints = 100
        };

        context.AddRange(team, membership);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync("/api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        result.Should().NotBeNull();
        result!.Teams.Should().NotBeNull();
    }
}




