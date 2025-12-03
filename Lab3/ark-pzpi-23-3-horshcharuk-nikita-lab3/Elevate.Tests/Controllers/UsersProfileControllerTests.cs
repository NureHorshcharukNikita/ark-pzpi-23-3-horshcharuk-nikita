using Elevate.Data;
using Elevate.Dtos.Users;
using Elevate.Entities;
using Elevate.Tests.Controllers;
using Elevate.Tests.TestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Elevate.Tests.Controllers;

public class UsersProfileControllerTests : ControllerTestBase
{
    public UsersProfileControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_ReturnsUserTeamsProfile()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var client = await CreateAuthenticatedClientAsync($"testuser_{uniqueId}");
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var user = new User
        {
            Login = $"profileuser_{uniqueId}",
            Email = $"profileuser_{uniqueId}@test.com",
            FirstName = "Profile",
            LastName = "User",
            PasswordHash = "hash"
        };

        var team = new Team { Name = $"Test Team_{uniqueId}" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var member = new TeamMember
        {
            Team = team,
            User = user,
            TeamPoints = 200,
            TeamLevel = level
        };

        context.Users.Add(user);
        context.Teams.Add(team);
        context.TeamLevels.Add(level);
        context.TeamMembers.Add(member);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"/api/users/{user.UserID}/teams-profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var profile = await response.Content.ReadFromJsonAsync<UserTeamsProfileDto>();
        profile.Should().NotBeNull();
        profile!.UserId.Should().Be(user.UserID);
        profile.FullName.Should().Be("Profile User");
        // Filter to only our team to avoid interference from parallel tests
        var ourTeams = profile.Teams.Where(t => t.TeamId == team.TeamID).ToList();
        ourTeams.Should().HaveCount(1);
        ourTeams[0].TeamName.Should().Be($"Test Team_{uniqueId}");
        ourTeams[0].TeamPoints.Should().Be(200);
    }
}

