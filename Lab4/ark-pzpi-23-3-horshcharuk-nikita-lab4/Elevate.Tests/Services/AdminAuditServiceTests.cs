using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Admin;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class AdminAuditServiceTests
{
    [Fact]
    public async Task GetEventsAsync_ReturnsEvents()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminAuditService(context);

        var team = new Team { Name = "Team" };
        var user = new User
        {
            Login = "user",
            Email = "user@test.com",
            FirstName = "User",
            LastName = "Name",
            PasswordHash = "hash"
        };

        var actionType = new ActionType
        {
            Team = team,
            Code = "CODE1",
            Name = "Action",
            DefaultPoints = 10
        };

        var event1 = new ActionEvent
        {
            Team = team,
            User = user,
            ActionType = actionType,
            SourceType = "User",
            PointsAwarded = 10,
            OccurredAt = DateTime.UtcNow.AddDays(-1)
        };

        var event2 = new ActionEvent
        {
            Team = team,
            User = user,
            ActionType = actionType,
            SourceType = "Manager",
            PointsAwarded = 20,
            OccurredAt = DateTime.UtcNow
        };

        context.Teams.Add(team);
        context.Users.Add(user);
        context.ActionTypes.Add(actionType);
        context.ActionEvents.AddRange(event1, event2);
        await context.SaveChangesAsync();

        // Act
        var events = await service.GetEventsAsync(null, null, null, null, null, CancellationToken.None);

        // Assert
        events.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetEventsAsync_WithTeamIdFilter_ReturnsFilteredEvents()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminAuditService(context);

        var team1 = new Team { Name = "Team 1" };
        var team2 = new Team { Name = "Team 2" };
        var user = new User
        {
            Login = "user",
            Email = "user@test.com",
            FirstName = "User",
            LastName = "Name",
            PasswordHash = "hash"
        };

        var actionType1 = new ActionType { Team = team1, Code = "CODE1", Name = "Action", DefaultPoints = 10 };
        var actionType2 = new ActionType { Team = team2, Code = "CODE2", Name = "Action", DefaultPoints = 10 };

        var event1 = new ActionEvent
        {
            Team = team1,
            User = user,
            ActionType = actionType1,
            SourceType = "User",
            PointsAwarded = 10
        };

        var event2 = new ActionEvent
        {
            Team = team2,
            User = user,
            ActionType = actionType2,
            SourceType = "User",
            PointsAwarded = 10
        };

        context.Teams.AddRange(team1, team2);
        context.Users.Add(user);
        context.ActionTypes.AddRange(actionType1, actionType2);
        context.ActionEvents.AddRange(event1, event2);
        await context.SaveChangesAsync();

        // Act
        var events = await service.GetEventsAsync(team1.TeamID, null, null, null, null, CancellationToken.None);

        // Assert
        events.Should().HaveCount(1);
        events[0].TeamID.Should().Be(team1.TeamID);
    }

    [Fact]
    public async Task GetDeviceScansAsync_ReturnsScans()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminAuditService(context);

        var team = new Team { Name = "Team" };
        var user = new User
        {
            Login = "user",
            Email = "user@test.com",
            FirstName = "User",
            LastName = "Name",
            PasswordHash = "hash"
        };

        var device = new Device
        {
            Name = "Device",
            Team = team,
            DeviceKey = "key",
            IsActive = true
        };

        var scan1 = new DeviceScan
        {
            Device = device,
            Team = team,
            User = user,
            ScannedAt = DateTime.UtcNow.AddDays(-1)
        };

        var scan2 = new DeviceScan
        {
            Device = device,
            Team = team,
            User = user,
            ScannedAt = DateTime.UtcNow
        };

        context.Teams.Add(team);
        context.Users.Add(user);
        context.Devices.Add(device);
        context.DeviceScans.AddRange(scan1, scan2);
        await context.SaveChangesAsync();

        // Act
        var scans = await service.GetDeviceScansAsync(null, null, null, null, CancellationToken.None);

        // Assert
        scans.Should().HaveCount(2);
    }
}

