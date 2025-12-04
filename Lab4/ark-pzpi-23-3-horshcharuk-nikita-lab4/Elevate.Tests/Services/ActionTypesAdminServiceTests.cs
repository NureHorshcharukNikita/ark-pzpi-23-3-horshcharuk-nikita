using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Admin;
using Elevate.Tests.TestUtilities;

namespace Elevate.Tests.Services;

public class ActionTypesAdminServiceTests
{
    [Fact]
    public async Task CreateActionTypeAsync_CreatesActionType()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new ActionTypesAdminService(context);

        var team = new Team { Name = "Team" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        // Act
        var actionType = await service.CreateActionTypeAsync(
            team.TeamID, "CODE1", "Action Name", "Description", 50, "Category", true, CancellationToken.None);

        // Assert
        actionType.Should().NotBeNull();
        actionType.Code.Should().Be("CODE1");
        actionType.Name.Should().Be("Action Name");
        actionType.DefaultPoints.Should().Be(50);
        actionType.IsActive.Should().BeTrue();
    }
}

