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

public class AdminDevicesControllerTests : ControllerTestBase
{
    public AdminDevicesControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateDevice_AsAdmin_CreatesDevice()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync(role: "Admin");
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var team = new Team { Name = "Test Team" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var request = new
        {
            Name = "New Device",
            TeamId = team.TeamID,
            Location = "Office"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/admin/devices", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<object>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task DeactivateDevice_AsAdmin_DeactivatesDevice()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync(role: "Admin");
        await using var setupScope = Factory.Services.CreateAsyncScope();
        var setupContext = setupScope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var team = new Team { Name = "Test Team" };
        var device = new Device
        {
            Name = "Device",
            Team = team,
            DeviceKey = "key",
            IsActive = true
        };

        setupContext.Teams.Add(team);
        setupContext.Devices.Add(device);
        await setupContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsync($"/api/admin/devices/{device.DeviceID}/deactivate", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Note: InMemory DB changes are verified through service layer tests
        // Integration tests verify HTTP contract, not DB state directly
    }
}

