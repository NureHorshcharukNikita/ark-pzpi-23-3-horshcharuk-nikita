using Elevate.Data;
using Elevate.Dtos.Users;
using Elevate.Entities;
using Elevate.Services.Users;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class UserProfileServiceTests
{
    [Fact]
    public async Task GetUserTeamsProfileAsync_ReturnsUserProfileWithTeams()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new UserProfileService(context);

        var user = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash"
        };

        var team1 = new Team { Name = "Backend Team" };
        var team2 = new Team { Name = "Frontend Team" };

        var level1 = new TeamLevel
        {
            Team = team1,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var level2 = new TeamLevel
        {
            Team = team2,
            Name = "Rookie",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var member1 = new TeamMember
        {
            Team = team1,
            User = user,
            TeamPoints = 300,
            TeamLevel = level1
        };

        var member2 = new TeamMember
        {
            Team = team2,
            User = user,
            TeamPoints = 150,
            TeamLevel = level2
        };

        context.AddRange(user, team1, team2, level1, level2, member1, member2);
        await context.SaveChangesAsync();

        // Act
        var profile = await service.GetUserTeamsProfileAsync(user.UserID, CancellationToken.None);

        // Assert
        profile.Should().NotBeNull();
        profile.UserId.Should().Be(user.UserID);
        profile.FullName.Should().Be("John Doe");
        profile.Teams.Should().HaveCount(2);
        profile.Teams.Should().Contain(t => t.TeamName == "Backend Team" && t.TeamPoints == 300);
        profile.Teams.Should().Contain(t => t.TeamName == "Frontend Team" && t.TeamPoints == 150);
    }

    [Fact]
    public async Task GetUserTeamsProfileAsync_IncludesBadges()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new UserProfileService(context);

        var user = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash"
        };

        var team = new Team { Name = "Backend Team" };
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
            TeamPoints = 300,
            TeamLevel = level
        };

        var badge1 = new TeamBadge
        {
            Team = team,
            Code = "BADGE1",
            Name = "First Badge",
            ConditionType = "TotalPoints",
            ConditionValue = 200
        };

        var badge2 = new TeamBadge
        {
            Team = team,
            Code = "BADGE2",
            Name = "Second Badge",
            ConditionType = "TotalPoints",
            ConditionValue = 300
        };

        var userBadge1 = new UserTeamBadge
        {
            User = user,
            Team = team,
            TeamBadge = badge1
        };

        var userBadge2 = new UserTeamBadge
        {
            User = user,
            Team = team,
            TeamBadge = badge2
        };

        context.AddRange(user, team, level, member, badge1, badge2, userBadge1, userBadge2);
        await context.SaveChangesAsync();

        // Act
        var profile = await service.GetUserTeamsProfileAsync(user.UserID, CancellationToken.None);

        // Assert
        profile.Should().NotBeNull();
        var teamInfo = profile.Teams.First(t => t.TeamId == team.TeamID);
        teamInfo.Badges.Should().HaveCount(2);
        teamInfo.Badges.Should().Contain("First Badge");
        teamInfo.Badges.Should().Contain("Second Badge");
    }

    [Fact]
    public async Task GetUserTeamsProfileAsync_WithNonExistentUser_ThrowsException()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new UserProfileService(context);

        // Act
        var act = () => service.GetUserTeamsProfileAsync(999, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*User not found*");
    }
}

