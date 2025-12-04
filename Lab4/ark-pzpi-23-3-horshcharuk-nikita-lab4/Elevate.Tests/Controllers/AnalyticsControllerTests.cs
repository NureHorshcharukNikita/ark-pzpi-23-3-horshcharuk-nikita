using Elevate.Dtos.Analytics;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Elevate.Tests.Controllers;

public class AnalyticsControllerTests : ControllerTestBase
{
    public AnalyticsControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Overview_WithValidTeamId_ReturnsOk()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync(role: "Manager");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 100,
            OrderIndex = 1
        };
        var badge = new TeamBadge
        {
            Team = team,
            Code = "BADGE1",
            Name = "Badge 1",
            ConditionType = "PointsReached",
            ConditionValue = 100
        };

        context.AddRange(team, level, badge);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/analytics/overview?teamId={team.TeamID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TeamAnalyticsDto>();
        result.Should().NotBeNull();
        result!.TeamId.Should().Be(team.TeamID);
        result.TeamName.Should().Be("Backend");
    }

    [Fact]
    public async Task Overview_WithInvalidTeamId_ReturnsNotFound()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync(role: "Manager");

        // Act
        var response = await client.GetAsync("/api/analytics/overview?teamId=99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Overview_WithoutManagerRole_ReturnsForbidden()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync(role: "User");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        context.Add(team);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/analytics/overview?teamId={team.TeamID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Overview_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync("/api/analytics/overview?teamId=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Overview_WithAdminRole_ReturnsOk()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync(role: "Admin");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        context.Add(team);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/analytics/overview?teamId={team.TeamID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Overview_WithDateRange_ReturnsOk()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync(role: "Manager");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        context.Add(team);
        await context.SaveChangesAsync();

        var from = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
        var to = DateTime.UtcNow.ToString("yyyy-MM-dd");

        // Act
        var response = await client.GetAsync($"/api/analytics/overview?teamId={team.TeamID}&from={from}&to={to}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TeamAnalyticsDto>();
        result.Should().NotBeNull();
    }
}




