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

public class AdminGamificationControllerTests : ControllerTestBase
{
    public AdminGamificationControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateLevel_AsAdmin_CreatesLevel()
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
            Name = "Legend",
            RequiredPoints = 500,
            OrderIndex = 3
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/admin/levels/teams/{team.TeamID}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var level = await response.Content.ReadFromJsonAsync<TeamLevel>();
        level.Should().NotBeNull();
        level!.Name.Should().Be("Legend");
    }

    [Fact]
    public async Task CreateBadge_AsAdmin_CreatesBadge()
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
            Code = "BADGE1",
            Name = "Badge Name",
            Description = "Description",
            IconCode = "icon",
            ConditionType = "TotalPoints",
            ConditionValue = 200
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/admin/badges/teams/{team.TeamID}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var badge = await response.Content.ReadFromJsonAsync<TeamBadge>();
        badge.Should().NotBeNull();
        badge!.Code.Should().Be("BADGE1");
    }
}

