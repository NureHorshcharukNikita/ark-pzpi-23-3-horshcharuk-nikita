using Elevate.Dtos.Teams;
using Elevate.Entities;
using Elevate.Services;
using Elevate.Services.Teams;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class TeamServiceTests
{
    [Fact]
    public async Task GetLeaderboardAsync_ReturnsOrderedMembersWithRanks()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();

        var (service, team, topUser, secondUser) =
            await CreateTeamServiceWithLeaderboardDataAsync(context);

        // Act
        var leaderboard = await service.GetLeaderboardAsync(team.TeamID, CancellationToken.None);

        // Assert
        leaderboard.Should().HaveCount(2);
        leaderboard.First().UserId.Should().Be(topUser.UserID);
        leaderboard.First().Rank.Should().Be(1);
        leaderboard.Last().Rank.Should().Be(2);
    }

    [Fact]
    public async Task CreateLevelAsync_PersistsLevelForTeam()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new TeamService(context);

        var team = new Team { Name = "Backend" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var dto = new CreateTeamLevelDto
        {
            Name = "Legend",
            RequiredPoints = 500,
            OrderIndex = 3
        };

        // Act
        var level = await service.CreateLevelAsync(team.TeamID, dto, CancellationToken.None);

        // Assert
        level.Name.Should().Be("Legend");
        (await context.TeamLevels.CountAsync()).Should().Be(1);
    }

    private static async Task<(TeamService service, Team team, User topUser, User secondUser)>
        CreateTeamServiceWithLeaderboardDataAsync(Elevate.Data.ElevateDbContext context)
    {
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
            Login = "john",
            Email = "john@elevate",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash"
        };

        var user2 = new User
        {
            Login = "anna",
            Email = "anna@elevate",
            FirstName = "Anna",
            LastName = "Smith",
            PasswordHash = "hash"
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

        var service = new TeamService(context);

        return (service, team, user1, user2);
    }
}
