using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Admin;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class AdminDeviceServiceTests
{
    [Fact]
    public async Task CreateDeviceAsync_CreatesDevice()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminDeviceService(context);

        var team = new Team { Name = "Team" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        // Act
        var device = await service.CreateDeviceAsync("Device Name", team.TeamID, "Location", CancellationToken.None);

        // Assert
        device.Should().NotBeNull();
        device.Name.Should().Be("Device Name");
        device.TeamID.Should().Be(team.TeamID);
        device.Location.Should().Be("Location");
        device.DeviceKey.Should().NotBeNullOrWhiteSpace();
        device.IsActive.Should().BeTrue();

        var deviceInDb = await context.Devices.FindAsync(device.DeviceID);
        deviceInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task SetDeviceActiveAsync_UpdatesDeviceActiveStatus()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminDeviceService(context);

        var team = new Team { Name = "Team" };
        var device = new Device
        {
            Name = "Device",
            Team = team,
            DeviceKey = "key",
            IsActive = true
        };

        context.Teams.Add(team);
        context.Devices.Add(device);
        await context.SaveChangesAsync();

        // Act
        await service.SetDeviceActiveAsync(device.DeviceID, false, CancellationToken.None);

        // Assert
        var updatedDevice = await context.Devices.FindAsync(device.DeviceID);
        updatedDevice!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllDevicesAsync_ReturnsAllDevices()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminDeviceService(context);

        var team = new Team { Name = "Team" };
        var device1 = new Device
        {
            Name = "Device 1",
            Team = team,
            DeviceKey = "key1",
            IsActive = true
        };

        var device2 = new Device
        {
            Name = "Device 2",
            Team = team,
            DeviceKey = "key2",
            IsActive = false
        };

        context.Teams.Add(team);
        context.Devices.AddRange(device1, device2);
        await context.SaveChangesAsync();

        // Act
        var devices = await service.GetAllDevicesAsync(CancellationToken.None);

        // Assert
        devices.Should().HaveCount(2);
        devices.Should().Contain(d => d.Name == "Device 1");
        devices.Should().Contain(d => d.Name == "Device 2");
    }
}

