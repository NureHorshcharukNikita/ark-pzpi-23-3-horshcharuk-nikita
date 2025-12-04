using Elevate.Data;
using Elevate.Dtos.Actions;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Elevate.Tests.Controllers;

public class TeamActionsControllerTests : ControllerTestBase
{
    public TeamActionsControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var team = new Team { Name = "Test Team" };
        var user = new User
        {
            Login = "actionuser",
            Email = "actionuser@test.com",
            FirstName = "Action",
            LastName = "User",
            PasswordHash = "hash"
        };

        var level = new TeamLevel
        {
            Team = team,
            Name = "Rookie",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var member = new TeamMember
        {
            Team = team,
            User = user,
            TeamPoints = 0,
            TeamLevel = level
        };

        var actionType = new ActionType
        {
            Team = team,
            Code = "TEST_ACTION",
            Name = "Test Action",
            DefaultPoints = 50,
            IsActive = true
        };

        context.Teams.Add(team);
        context.Users.Add(user);
        context.TeamLevels.Add(level);
        context.TeamMembers.Add(member);
        context.ActionTypes.Add(actionType);
        await context.SaveChangesAsync();

        var request = new
        {
            UserId = user.UserID,
            ActionTypeId = actionType.ActionTypeID,
            SourceType = "User",
            SourceUserId = (int?)null,
            Comment = "Test comment",
            OccurredAt = (DateTime?)null
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/teams/{team.TeamID}/actions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ActionEventResultDto>();
        result.Should().NotBeNull();
        result!.PointsAwarded.Should().Be(50);
        result.TotalTeamPoints.Should().Be(50);
    }
}

