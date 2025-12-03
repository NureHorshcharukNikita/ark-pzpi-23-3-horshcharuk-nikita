using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Admin;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class TeamLevelsAdminServiceTests
{
    [Fact]
    public async Task CreateTeamLevelAsync_CreatesLevel()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new TeamLevelsAdminService(context);

        var team = new Team { Name = "Team" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        // Act
        var level = await service.CreateTeamLevelAsync(team.TeamID, "Legend", 500, 3, CancellationToken.None);

        // Assert
        level.Should().NotBeNull();
        level.Name.Should().Be("Legend");
        level.RequiredPoints.Should().Be(500);
        level.OrderIndex.Should().Be(3);

        var levelInDb = await context.TeamLevels.FindAsync(level.TeamLevelID);
        levelInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTeamLevelAsync_UpdatesLevel()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new TeamLevelsAdminService(context);

        var team = new Team { Name = "Team" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Old Name",
            RequiredPoints = 100,
            OrderIndex = 1
        };

        context.Teams.Add(team);
        context.TeamLevels.Add(level);
        await context.SaveChangesAsync();

        // Act
        await service.UpdateTeamLevelAsync(level.TeamLevelID, "New Name", 200, 2, CancellationToken.None);

        // Assert
        var updatedLevel = await context.TeamLevels.FindAsync(level.TeamLevelID);
        updatedLevel!.Name.Should().Be("New Name");
        updatedLevel.RequiredPoints.Should().Be(200);
        updatedLevel.OrderIndex.Should().Be(2);
    }

    [Fact]
    public async Task GetTeamLevelsAsync_ReturnsLevelsForTeam()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new TeamLevelsAdminService(context);

        var team1 = new Team { Name = "Team 1" };
        var team2 = new Team { Name = "Team 2" };

        var level1 = new TeamLevel { Team = team1, Name = "Level 1", RequiredPoints = 0, OrderIndex = 1 };
        var level2 = new TeamLevel { Team = team1, Name = "Level 2", RequiredPoints = 100, OrderIndex = 2 };
        var level3 = new TeamLevel { Team = team2, Name = "Level 3", RequiredPoints = 0, OrderIndex = 1 };

        context.Teams.AddRange(team1, team2);
        context.TeamLevels.AddRange(level1, level2, level3);
        await context.SaveChangesAsync();

        // Act
        var levels = await service.GetTeamLevelsAsync(team1.TeamID, CancellationToken.None);

        // Assert
        levels.Should().HaveCount(2);
        levels.Should().OnlyContain(l => l.TeamID == team1.TeamID);
    }
}

