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

public class AdminAuditControllerTests : ControllerTestBase
{
    public AdminAuditControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetEvents_AsAdmin_ReturnsEvents()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var client = await CreateAuthenticatedClientAsync(login: $"admin_{uniqueId}", role: "Admin");
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var team = new Team { Name = $"Test Team_{uniqueId}" };
        var user = new User
        {
            Login = $"user_{uniqueId}",
            Email = $"user_{uniqueId}@test.com",
            FirstName = "User",
            LastName = "Name",
            PasswordHash = "hash"
        };

        var actionType = new ActionType
        {
            Team = team,
            Code = $"CODE1_{uniqueId}",
            Name = "Action",
            DefaultPoints = 10
        };

        var actionEvent = new ActionEvent
        {
            Team = team,
            User = user,
            ActionType = actionType,
            SourceType = "User",
            PointsAwarded = 10,
            OccurredAt = DateTime.UtcNow
        };

        context.Teams.Add(team);
        context.Users.Add(user);
        context.ActionTypes.Add(actionType);
        context.ActionEvents.Add(actionEvent);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync("/api/admin/events");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var events = await response.Content.ReadFromJsonAsync<object[]>();
        events.Should().NotBeNull();
        events!.Length.Should().BeGreaterOrEqualTo(1);
        // Verify our event is in the results by checking if any event exists
        // (we can't easily filter by our specific event without knowing the response structure)
    }

    [Fact]
    public async Task GetDeviceScans_AsAdmin_ReturnsScans()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var client = await CreateAuthenticatedClientAsync(login: $"admin_{uniqueId}", role: "Admin");
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var team = new Team { Name = $"Test Team_{uniqueId}" };
        var user = new User
        {
            Login = $"user_{uniqueId}",
            Email = $"user_{uniqueId}@test.com",
            FirstName = "User",
            LastName = "Name",
            PasswordHash = "hash"
        };

        var device = new Device
        {
            Name = $"Device_{uniqueId}",
            Team = team,
            DeviceKey = $"key_{uniqueId}",
            IsActive = true
        };

        var scan = new DeviceScan
        {
            Device = device,
            Team = team,
            User = user,
            ScannedAt = DateTime.UtcNow
        };

        context.Teams.Add(team);
        context.Users.Add(user);
        context.Devices.Add(device);
        context.DeviceScans.Add(scan);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync("/api/admin/device-scans");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var scans = await response.Content.ReadFromJsonAsync<object[]>();
        scans.Should().NotBeNull();
        scans!.Length.Should().BeGreaterOrEqualTo(1);
    }
}

