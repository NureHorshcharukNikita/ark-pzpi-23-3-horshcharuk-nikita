using Elevate.Dtos.Actions;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using Microsoft.AspNetCore.Identity;
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

        var team = new Team { Name = "Backend" };
        var actionType = new ActionType
        {
            Team = team,
            Code = "DEPLOY",
            Name = "Deploy",
            DefaultPoints = 10
        };
        var existingUser = await context.Users.FindAsync(user.UserID);
        var membership = new TeamMember
        {
            Team = team,
            User = existingUser!,
            TeamPoints = 0
        };

        context.AddRange(team, actionType, membership);
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
        var (user, client) = await CreateAuthenticatedUserAndClientAsync();
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        var actionType = new ActionType
        {
            Team = team,
            Code = "DEPLOY",
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
        var actionType = new ActionType
        {
            Team = team,
            Code = "DEPLOY",
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
        result!.Should().HaveCount(1);
        result.First().UserId.Should().Be(user1.UserID);
    }

    [Fact]
    public async Task Query_WithTeamId_ReturnsFilteredEvents()
    {
        // Arrange
        var (user, client) = await CreateAuthenticatedUserAndClientAsync();
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team1 = new Team { Name = "Backend" };
        var team2 = new Team { Name = "Frontend" };
        var actionType1 = new ActionType
        {
            Team = team1,
            Code = "DEPLOY",
            Name = "Deploy",
            DefaultPoints = 10
        };
        var actionType2 = new ActionType
        {
            Team = team2,
            Code = "DEPLOY",
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
        result!.Should().HaveCount(1);
        result.First().TeamId.Should().Be(team1.TeamID);
    }

    [Fact]
    public async Task Query_WithoutFilters_ReturnsAllEvents()
    {
        // Arrange
        var (user, client) = await CreateAuthenticatedUserAndClientAsync();
        await using var scope = Factory.Services.CreateAsyncScope(); var context = scope.ServiceProvider.GetRequiredService<Elevate.Data.ElevateDbContext>();

        var team = new Team { Name = "Backend" };
        var actionType = new ActionType
        {
            Team = team,
            Code = "DEPLOY",
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
        result!.Count.Should().BeGreaterOrEqualTo(2);
    }
}



