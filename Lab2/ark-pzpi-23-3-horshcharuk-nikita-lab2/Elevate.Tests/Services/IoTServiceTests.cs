using Elevate.Data;
using Elevate.Dtos.IoT;
using Elevate.Entities;
using Elevate.Services.Actions;
using Elevate.Services.Gamification;
using Elevate.Services.IoT;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class IoTServiceTests
{
    [Fact]
    public async Task ProcessScanAsync_ReturnsProfileSnapshot_AndCreatesDeviceScan()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();

        var (service, user) = await CreateIoTServiceWithDataAsync(context);

        var request = new DeviceScanRequestDto
        {
            DeviceKey = "device-key-001",
            UserId = user.UserID,
            ActionCode = "START_SHIFT"
        };

        // Act
        var response = await service.ProcessScanAsync(request, CancellationToken.None);

        // Assert
        response.TeamPoints.Should().Be(225);
        response.TeamLevel.Should().Be("Pro");
        response.FullName.Should().Contain("John");

        (await context.DeviceScans.CountAsync()).Should().Be(1);
        (await context.ActionEvents.CountAsync()).Should().Be(1);
    }

    private static async Task<(IoTService service, User user)>
        CreateIoTServiceWithDataAsync(ElevateDbContext context)
    {
        var team = new Team { Name = "Backend" };
        var level = new TeamLevel
        {
            Team = team,
            Name = "Pro",
            RequiredPoints = 0,
            OrderIndex = 1
        };

        var user = new User
        {
            Login = "john",
            Email = "john@elevate",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash"
        };

        var member = new TeamMember
        {
            Team = team,
            User = user,
            TeamPoints = 220,
            TeamLevel = level
        };

        var actionType = new ActionType
        {
            Team = team,
            Code = "START_SHIFT",
            Name = "Start Shift",
            DefaultPoints = 5
        };

        var device = new Device
        {
            Team = team,
            Name = "Panel",
            DeviceKey = "device-key-001"
        };

        context.AddRange(team, level, user, member, actionType, device);
        await context.SaveChangesAsync();

        var gamification = new GamificationService(context);
        var actionService = new ActionEventService(context, gamification);
        var service = new IoTService(context, actionService);

        return (service, user);
    }
}
