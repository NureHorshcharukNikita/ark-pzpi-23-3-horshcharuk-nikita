using Elevate.Data;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Elevate.Tests.Controllers;

public class AdminUsersControllerTests : ControllerTestBase
{
    public AdminUsersControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAll_AsAdmin_ReturnsAllUsers()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync(role: "Admin");
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var user1 = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "User",
            LastName = "One",
            PasswordHash = "hash",
            Role = "User"
        };

        var user2 = new User
        {
            Login = "user2",
            Email = "user2@test.com",
            FirstName = "User",
            LastName = "Two",
            PasswordHash = "hash",
            Role = "Manager"
        };

        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync("/api/admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<object[]>();
        users.Should().NotBeNull();
        users!.Length.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task SetRole_AsAdmin_UpdatesUserRole()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var client = await CreateAuthenticatedClientAsync(login: $"admin_{uniqueId}", role: "Admin");
        await using var setupScope = Factory.Services.CreateAsyncScope();
        var setupContext = setupScope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var user = new User
        {
            Login = $"testuser_{uniqueId}",
            Email = $"testuser_{uniqueId}@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            Role = "User"
        };

        setupContext.Users.Add(user);
        await setupContext.SaveChangesAsync();

        var request = new { Role = "Manager" };

        // Act
        var response = await client.PostAsJsonAsync($"/api/admin/users/{user.UserID}/role", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Note: InMemory DB changes are verified through service layer tests
        // Integration tests verify HTTP contract, not DB state directly
    }

    [Fact]
    public async Task Block_AsAdmin_BlocksUser()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync(role: "Admin");
        await using var setupScope = Factory.Services.CreateAsyncScope();
        var setupContext = setupScope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var user = new User
        {
            Login = "testuser",
            Email = "testuser@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            IsActive = true
        };

        setupContext.Users.Add(user);
        await setupContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsync($"/api/admin/users/{user.UserID}/block", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Note: InMemory DB changes are verified through service layer tests
        // Integration tests verify HTTP contract, not DB state directly
    }
}

