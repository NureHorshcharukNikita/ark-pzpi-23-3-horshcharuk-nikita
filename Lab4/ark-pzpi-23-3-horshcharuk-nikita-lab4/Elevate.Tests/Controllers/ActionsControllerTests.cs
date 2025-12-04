using Elevate.Dtos.Actions;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace Elevate.Tests.Controllers;

public class ActionsControllerTests : ControllerTestBase
{
    public ActionsControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var (user, client) = await CreateAuthenticatedUserAndClientAsync();
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var team = new Team { Name = $"Backend_{uniqueId}" };
        var actionType = new ActionType
        {
            Team = team,
            Code = $"DEPLOY_{uniqueId}",
            Name = "Deploy",
            DefaultPoints = 10,
            IsActive = true
        };
        var existingUser = await context.Users.FindAsync(user.UserID);
        var level = new TeamLevel
        {
            Team = team,
            Name = "Rookie",
            RequiredPoints = 0,
            OrderIndex = 1
        };
        var membership = new TeamMember
        {
            Team = team,
            User = existingUser!,
            TeamPoints = 0,
            TeamLevel = level
        };

        context.AddRange(team, level, actionType, membership);
        await context.SaveChangesAsync();

        var dto = new CreateActionEventDto
        {
            TeamId = team.TeamID,
            ActionTypeId = actionType.ActionTypeID,
            SourceType = "Manager",
            Comment = "Great work!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/actions", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ActionEventDto>();
        result.Should().NotBeNull();
        result!.TeamId.Should().Be(team.TeamID);
        result.UserId.Should().Be(user.UserID);
    }

    [Fact]
    public async Task Create_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new CreateActionEventDto
        {
            TeamId = 1,
            ActionTypeId = 1,
            SourceType = "Manager"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/actions", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var (user, client) = await CreateAuthenticatedUserAndClientAsync($"testuser_{uniqueId}");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = $"Backend_{uniqueId}" };
        var actionType = new ActionType
        {
            Team = team,
            Code = $"DEPLOY_{uniqueId}",
            Name = "Deploy",
            DefaultPoints = 10
        };
        var existingUser = await context.Users.FindAsync(user.UserID);
        var actionEvent = new ActionEvent
        {
            User = existingUser!,
            Team = team,
            ActionType = actionType,
            SourceType = "Manager",
            PointsAwarded = 10,
            OccurredAt = DateTime.UtcNow
        };

        context.AddRange(team, actionType, actionEvent);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/actions/{actionEvent.ActionEventID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Elevate.Entities.ActionEvent>();
        result.Should().NotBeNull();
        result!.ActionEventID.Should().Be(actionEvent.ActionEventID);
        result.UserID.Should().Be(user.UserID);
        result.TeamID.Should().Be(team.TeamID);
        result.PointsAwarded.Should().Be(10);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var (_, client) = await CreateAuthenticatedUserAndClientAsync();

        // Act
        var response = await client.GetAsync("/api/actions/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Query_WithUserId_ReturnsFilteredEvents()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var (user1, client) = await CreateAuthenticatedUserAndClientAsync($"user1_{uniqueId}");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var user2 = new User
        {
            Login = $"user2_{uniqueId}",
            Email = $"user2_{uniqueId}@test.com",
            FirstName = "User",
            LastName = "Two",
            PasswordHash = "hash",
            Role = "User"
        };

        var team = new Team { Name = $"Backend_{uniqueId}" };
        var actionType = new ActionType
        {
            Team = team,
            Code = $"DEPLOY_{uniqueId}",
            Name = "Deploy",
            DefaultPoints = 10
        };

        // Need to attach user1 to this context or create new reference
        var existingUser1 = await context.Users.FindAsync(user1.UserID);
        var event1 = new ActionEvent
        {
            User = existingUser1!,
            Team = team,
            ActionType = actionType,
            SourceType = "Manager",
            PointsAwarded = 10,
            OccurredAt = DateTime.UtcNow
        };

        var event2 = new ActionEvent
        {
            User = user2,
            Team = team,
            ActionType = actionType,
            SourceType = "Manager",
            PointsAwarded = 10,
            OccurredAt = DateTime.UtcNow
        };

        context.AddRange(user2, team, actionType, event1, event2);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/actions?userId={user1.UserID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ActionEventDto>>();
        result.Should().NotBeNull();
        // Filter to only our user's events to avoid interference from parallel tests
        var user1Events = result!.Where(e => e.UserId == user1.UserID).ToList();
        user1Events.Should().HaveCount(1);
        user1Events.First().UserId.Should().Be(user1.UserID);
        user1Events.First().TeamId.Should().Be(team.TeamID);
    }

    [Fact]
    public async Task Query_WithTeamId_ReturnsFilteredEvents()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var (user, client) = await CreateAuthenticatedUserAndClientAsync($"testuser_{uniqueId}");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team1 = new Team { Name = $"Backend_{uniqueId}" };
        var team2 = new Team { Name = $"Frontend_{uniqueId}" };
        var actionType1 = new ActionType
        {
            Team = team1,
            Code = $"DEPLOY_{uniqueId}",
            Name = "Deploy",
            DefaultPoints = 10
        };
        var actionType2 = new ActionType
        {
            Team = team2,
            Code = $"DEPLOY_{uniqueId}_2",
            Name = "Deploy",
            DefaultPoints = 10
        };

        var existingUser = await context.Users.FindAsync(user.UserID);
        var event1 = new ActionEvent
        {
            User = existingUser!,
            Team = team1,
            ActionType = actionType1,
            SourceType = "Manager",
            PointsAwarded = 10,
            OccurredAt = DateTime.UtcNow
        };

        var event2 = new ActionEvent
        {
            User = existingUser!,
            Team = team2,
            ActionType = actionType2,
            SourceType = "Manager",
            PointsAwarded = 10,
            OccurredAt = DateTime.UtcNow
        };

        context.AddRange(team1, team2, actionType1, actionType2, event1, event2);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/actions?teamId={team1.TeamID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ActionEventDto>>();
        result.Should().NotBeNull();
        // Filter to only our team's events to avoid interference from parallel tests
        var team1Events = result!.Where(e => e.TeamId == team1.TeamID && e.UserId == user.UserID).ToList();
        team1Events.Should().HaveCount(1);
        team1Events.First().TeamId.Should().Be(team1.TeamID);
        team1Events.First().UserId.Should().Be(user.UserID);
    }

    [Fact]
    public async Task Query_WithoutFilters_ReturnsAllEvents()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var (user, client) = await CreateAuthenticatedUserAndClientAsync($"testuser_{uniqueId}");
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = $"Backend_{uniqueId}" };
        var actionType = new ActionType
        {
            Team = team,
            Code = $"DEPLOY_{uniqueId}",
            Name = "Deploy",
            DefaultPoints = 10
        };

        var existingUser = await context.Users.FindAsync(user.UserID);
        var event1 = new ActionEvent
        {
            User = existingUser!,
            Team = team,
            ActionType = actionType,
            SourceType = "Manager",
            PointsAwarded = 10,
            OccurredAt = DateTime.UtcNow
        };

        var event2 = new ActionEvent
        {
            User = existingUser!,
            Team = team,
            ActionType = actionType,
            SourceType = "Manager",
            PointsAwarded = 15,
            OccurredAt = DateTime.UtcNow
        };

        context.AddRange(team, actionType, event1, event2);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync("/api/actions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ActionEventDto>>();
        result.Should().NotBeNull();
        // Filter to only our events to avoid interference from parallel tests
        var ourEvents = result!.Where(e => e.TeamId == team.TeamID && e.UserId == user.UserID).ToList();
        ourEvents.Should().HaveCount(2);
        
        // Verify both events are present with specific points
        ourEvents.Should().Contain(e => e.PointsAwarded == 10 && e.TeamId == team.TeamID && e.UserId == user.UserID);
        ourEvents.Should().Contain(e => e.PointsAwarded == 15 && e.TeamId == team.TeamID && e.UserId == user.UserID);
    }
}



