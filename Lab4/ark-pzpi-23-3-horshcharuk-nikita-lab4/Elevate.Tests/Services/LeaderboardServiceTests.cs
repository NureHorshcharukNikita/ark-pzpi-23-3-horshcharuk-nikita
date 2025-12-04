using Elevate.Data;
using Elevate.Dtos.Teams;
using Elevate.Entities;
using Elevate.Services.Leaderboard;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class LeaderboardServiceTests
{
    [Fact]
    public async Task GetTeamLeaderboardAsync_ReturnsOrderedMembersByPoints()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new LeaderboardService(context);

        var team = new Team { Name = "Backend Team" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var user1 = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash"
        };

        var user2 = new User
        {
            Login = "user2",
            Email = "user2@test.com",
            FirstName = "Anna",
            LastName = "Smith",
            PasswordHash = "hash"
        };

        var member1 = new TeamMember
        {
            Team = team,
            User = user1,
            TeamPoints = 300,
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

        // Act
        var leaderboard = await service.GetTeamLeaderboardAsync(team.TeamID, null, CancellationToken.None);

        // Assert
        leaderboard.Should().HaveCount(2);
        leaderboard[0].Position.Should().Be(1);
        leaderboard[0].UserId.Should().Be(user1.UserID);
        leaderboard[0].TeamPoints.Should().Be(300);
        leaderboard[0].FullName.Should().Be("John Doe");
        leaderboard[1].Position.Should().Be(2);
        leaderboard[1].UserId.Should().Be(user2.UserID);
        leaderboard[1].TeamPoints.Should().Be(150);
    }

    [Fact]
    public async Task GetTeamLeaderboardAsync_WithTopN_ReturnsLimitedResults()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new LeaderboardService(context);

        var team = new Team { Name = "Backend Team" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var users = Enumerable.Range(1, 5).Select(i => new User
        {
            Login = $"user{i}",
            Email = $"user{i}@test.com",
            FirstName = $"User{i}",
            LastName = "Test",
            PasswordHash = "hash"
        }).ToList();

        var members = users.Select((u, i) => new TeamMember
        {
            Team = team,
            User = u,
            TeamPoints = 500 - (i * 50),
            TeamLevel = level
        }).ToList();

        context.AddRange(team, level);
        context.AddRange(users);
        context.AddRange(members);
        await context.SaveChangesAsync();

        // Act
        var leaderboard = await service.GetTeamLeaderboardAsync(team.TeamID, 3, CancellationToken.None);

        // Assert
        leaderboard.Should().HaveCount(3);
        leaderboard[0].Position.Should().Be(1);
        leaderboard[1].Position.Should().Be(2);
        leaderboard[2].Position.Should().Be(3);
    }

    [Fact]
    public async Task GetTeamLeaderboardAsync_WithTeamLevel_IncludesLevelName()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new LeaderboardService(context);

        var team = new Team { Name = "Backend Team" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Legend",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var user = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash"
        };

        var member = new TeamMember
        {
            Team = team,
            User = user,
            TeamPoints = 300,
            TeamLevel = level
        };

        context.AddRange(team, level, user, member);
        await context.SaveChangesAsync();

        // Act
        var leaderboard = await service.GetTeamLeaderboardAsync(team.TeamID, null, CancellationToken.None);

        // Assert
        leaderboard.Should().HaveCount(1);
        leaderboard[0].TeamLevelName.Should().Be("Legend");
    }
}

