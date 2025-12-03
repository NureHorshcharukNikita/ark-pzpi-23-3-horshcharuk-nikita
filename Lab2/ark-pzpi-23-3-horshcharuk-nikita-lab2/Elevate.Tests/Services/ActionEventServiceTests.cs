using Elevate.Dtos.Actions;
using Elevate.Entities;
using Elevate.Services.Actions;
using Elevate.Services.Gamification;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class ActionEventServiceTests
{
    [Fact]
    public async Task CreateAsync_IncrementsPoints_AssignsLevelAndBadge()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();

        var (team, user, actionType, _) =
            await SeedTeamWithUserLevelAndBadgeAsync(context);

        var gamificationService = new GamificationService(context);
        var sut = new ActionEventService(context, gamificationService);

        var dto = new CreateActionEventDto
        {
            TeamId = team.TeamID,
            ActionTypeId = actionType.ActionTypeID,
            SourceType = "Manager",
            Comment = "Great deploy"
        };

        // Act
        var result = await sut.CreateAsync(dto, user.UserID, CancellationToken.None);

        // Assert
        result.TeamPoints.Should().Be(210);
        result.TeamLevel.Should().Be("Pro");
        result.NewTeamBadges.Should().ContainSingle(b => b.BadgeName == "Sprint Hero");
        (await context.UserTeamBadges.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task GetAsync_FiltersByUserAndTeam()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();

        var user = new User
        {
            Login = "john",
            Email = "john@elevate",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash"
        };

        var backendTeam = new Team { Name = "Backend" };
        var mobileTeam = new Team { Name = "Mobile" };

        var membership = new TeamMember
        {
            Team = backendTeam,
            User = user,
            TeamPoints = 100
        };

        var deployType = new ActionType
        {
            Team = backendTeam,
            Code = "DEPLOY",
            Name = "Deploy",
            DefaultPoints = 10
        };

        var hotfixType = new ActionType
        {
            Team = mobileTeam,
            Code = "HOTFIX",
            Name = "Hotfix",
            DefaultPoints = 15
        };

        context.AddRange(user, backendTeam, mobileTeam, membership, deployType, hotfixType);

        context.ActionEvents.Add(new ActionEvent
        {
            User = user,
            Team = backendTeam,
            ActionType = deployType,
            SourceType = "Manager",
            PointsAwarded = 10
        });

        context.ActionEvents.Add(new ActionEvent
        {
            User = user,
            Team = mobileTeam,
            ActionType = hotfixType,
            SourceType = "Manager",
            PointsAwarded = 15
        });

        await context.SaveChangesAsync();

        var gamificationService = new GamificationService(context);
        var sut = new ActionEventService(context, gamificationService);

        // Act
        var result = await sut.GetAsync(user.UserID, backendTeam.TeamID, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().TeamId.Should().Be(backendTeam.TeamID);
    }

    /// <summary>
    /// Готує дані для сценарію, де користувач добирає бали,
    /// отримує рівень і бейдж.
    /// </summary>
    private static async Task<(Team team, User user, ActionType actionType, TeamBadge badge)>
        SeedTeamWithUserLevelAndBadgeAsync(DbContext context)
    {
        var team = new Team { Name = "Backend" };

        var user = new User
        {
            Login = "john",
            Email = "john@elevate",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash"
        };

        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 200,
            OrderIndex = 1
        };

        var membership = new TeamMember
        {
            Team = team,
            User = user,
            TeamPoints = 190,
            TeamLevel = null
        };

        var actionType = new ActionType
        {
            Team = team,
            Code = "DEPLOY",
            Name = "Deploy",
            DefaultPoints = 20
        };

        var badge = new TeamBadge
        {
            Team = team,
            Code = "SPRINT_HERO",
            Name = "Sprint Hero",
            ConditionType = "PointsReached",
            ConditionValue = 200
        };

        context.AddRange(team, user, level, membership, actionType, badge);
        await context.SaveChangesAsync();

        return (team, user, actionType, badge);
    }
}
