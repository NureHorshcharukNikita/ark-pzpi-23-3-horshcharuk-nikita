using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Admin;
using Elevate.Tests.TestUtilities;

namespace Elevate.Tests.Services;

public class TeamBadgesAdminServiceTests
{
    [Fact]
    public async Task CreateTeamBadgeAsync_CreatesBadge()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new TeamBadgesAdminService(context);

        var team = new Team { Name = "Team" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        // Act
        var badge = await service.CreateTeamBadgeAsync(
            team.TeamID, "BADGE1", "Badge Name", "Description", "icon", "TotalPoints", 200, CancellationToken.None);

        // Assert
        badge.Should().NotBeNull();
        badge.Code.Should().Be("BADGE1");
        badge.Name.Should().Be("Badge Name");
        badge.ConditionType.Should().Be("TotalPoints");
        badge.ConditionValue.Should().Be(200);
    }
}

