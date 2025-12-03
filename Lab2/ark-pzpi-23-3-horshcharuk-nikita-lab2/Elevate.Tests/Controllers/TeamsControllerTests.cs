using Elevate.Dtos.Teams;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Elevate.Tests.Controllers;

public class TeamsControllerTests : ControllerTestBase
{
    public TeamsControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetTeams_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync();
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team1 = new Team { Name = "Backend" };
        var team2 = new Team { Name = "Frontend" };
        context.AddRange(team1, team2);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync("/api/teams");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<System.Collections.Generic.List<TeamDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetTeams_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync("/api/teams");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTeam_WithValidId_ReturnsOk()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync();
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend", Description = "Backend team" };
        context.Add(team);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/teams/{team.TeamID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TeamDetailDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(team.TeamID);
        result.Name.Should().Be("Backend");
    }

    [Fact]
    public async Task GetTeam_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync();

        // Act
        var response = await client.GetAsync("/api/teams/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMembers_WithValidTeamId_ReturnsOk()
    {
        // Arrange
        var (user1, client) = await CreateAuthenticatedUserAndClientAsync("user1");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var user2 = new User
        {
            Login = "user2",
            Email = "user2@test.com",
            FirstName = "User",
            LastName = "Two",
            PasswordHash = "hash",
            Role = "User"
        };

        var team = new Team { Name = "Backend" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        // Need to attach user1 to this context or create new reference
        var existingUser1 = await context.Users.FindAsync(user1.UserID);
        var member1 = new TeamMember
        {
            Team = team,
            User = existingUser1!,
            TeamPoints = 100,
            TeamLevel = level
        };
        var member2 = new TeamMember
        {
            Team = team,
            User = user2,
            TeamPoints = 150,
            TeamLevel = level
        };

        context.AddRange(user2, team, level, member1, member2);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/teams/{team.TeamID}/members");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<TeamMemberDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetMembers_WithInvalidTeamId_ReturnsEmptyList()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync();

        // Act
        var response = await client.GetAsync("/api/teams/99999/members");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<TeamMemberDto>>();
        result.Should().NotBeNull();
        result!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLeaderboard_WithValidTeamId_ReturnsOk()
    {
        // Arrange
        var (user1, client) = await CreateAuthenticatedUserAndClientAsync("user1");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var user2 = new User
        {
            Login = "user2",
            Email = "user2@test.com",
            FirstName = "User",
            LastName = "Two",
            PasswordHash = "hash",
            Role = "User"
        };

        var team = new Team { Name = "Backend" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var existingUser1 = await context.Users.FindAsync(user1.UserID);
        var member1 = new TeamMember
        {
            Team = team,
            User = existingUser1!,
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

        context.AddRange(user2, team, level, member1, member2);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/teams/{team.TeamID}/leaderboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<LeaderboardEntryDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterOrEqualTo(2);
        result.First().Rank.Should().Be(1);
    }

    [Fact]
    public async Task GetLeaderboard_WithInvalidTeamId_ReturnsEmptyList()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync();

        // Act
        var response = await client.GetAsync("/api/teams/99999/leaderboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<LeaderboardEntryDto>>();
        result.Should().NotBeNull();
        result!.Should().BeEmpty();
    }
}




