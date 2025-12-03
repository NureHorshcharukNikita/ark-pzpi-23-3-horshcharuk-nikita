using Elevate.Dtos.IoT;
using Elevate.Dtos.Teams;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Elevate.Tests.Controllers;

public class IoTControllerTests : ControllerTestBase
{
    public IoTControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Scan_WithValidDeviceAndUserId_ReturnsOk()
    {
        // Arrange
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var user = new User
        {
            Login = $"iotuser_{uniqueId}",
            Email = $"iot_{uniqueId}@test.com",
            FirstName = "IoT",
            LastName = "User",
            PasswordHash = "hash",
            Role = "User"
        };
        var membership = new TeamMember
        {
            Team = team,
            User = user,
            TeamPoints = 100,
            TeamLevel = level
        };
        var device = new Device
        {
            Team = team,
            Name = "Panel",
            DeviceKey = $"device-key-{uniqueId}",
            IsActive = true
        };

        context.AddRange(team, level, user, membership, device);
        await context.SaveChangesAsync();

        var request = new
        {
            DeviceKey = $"device-key-{uniqueId}",
            UserId = user.UserID
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/iot/scan", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Elevate.Dtos.IoT.IotScanResultDto>();
        result.Should().NotBeNull();
        result!.FullName.Should().Contain("IoT");
        result.TeamId.Should().Be(team.TeamID);
        result.UserId.Should().Be(user.UserID);
        result.TeamPoints.Should().Be(100);
    }

    [Fact]
    public async Task Scan_WithInvalidDeviceKey_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            DeviceKey = "invalid-device-key",
            UserId = 1
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/iot/scan", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Scan_WithUnknownUserId_ReturnsBadRequest()
    {
        // Arrange
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        var device = new Device
        {
            Team = team,
            Name = "Panel",
            DeviceKey = "device-key-001",
            IsActive = true
        };
        context.AddRange(team, device);
        await context.SaveChangesAsync();

        var request = new
        {
            DeviceKey = "device-key-001",
            UserId = 99999 // Non-existent user
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/iot/scan", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Leaderboard_WithValidTeamId_ReturnsOk()
    {
        // Arrange
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };
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
            Role = "User"
        };
        var member1 = new TeamMember
        {
            Team = team,
            User = user1,
            TeamPoints = 200,
            TeamLevel = level
        };
        var member2 = new TeamMember
        {
            Team = team,
            User = user2,
            TeamPoints = 150,
            TeamLevel = level
        };

        context.AddRange(team, level, user1, user2, member1, member2);
        await context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/iot/leaderboard?teamId={team.TeamID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<System.Collections.Generic.List<LeaderboardEntryDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task Leaderboard_WithInvalidTeamId_ReturnsEmptyList()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync("/api/iot/leaderboard?teamId=99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<System.Collections.Generic.List<LeaderboardEntryDto>>();
        result.Should().NotBeNull();
        result!.Should().BeEmpty();
    }

    [Fact]
    public async Task Scan_WithUserNotInTeam_ReturnsBadRequest()
    {
        // Arrange
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        var user = new User
        {
            Login = "iotuser",
            Email = "iot@test.com",
            FirstName = "IoT",
            LastName = "User",
            PasswordHash = "hash",
            Role = "User"
        };
        var device = new Device
        {
            Team = team,
            Name = "Panel",
            DeviceKey = "device-key-002",
            IsActive = true
        };

        context.AddRange(team, user, device);
        await context.SaveChangesAsync();

        var request = new
        {
            DeviceKey = "device-key-002",
            UserId = user.UserID
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/iot/scan", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}




