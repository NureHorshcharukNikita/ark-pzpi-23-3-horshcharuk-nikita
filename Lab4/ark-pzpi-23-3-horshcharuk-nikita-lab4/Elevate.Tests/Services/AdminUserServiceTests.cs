using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Admin;
using Elevate.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Tests.Services;

public class AdminUserServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminUserService(context);

        var user1 = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash",
            Role = "User"
        };

        var user2 = new User
        {
            Login = "user2",
            Email = "user2@test.com",
            FirstName = "Anna",
            LastName = "Smith",
            PasswordHash = "hash",
            Role = "Manager"
        };

        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        // Act
        var users = await service.GetAllAsync(CancellationToken.None);

        // Assert
        users.Should().HaveCount(2);
        users.Should().Contain(u => u.Login == "user1");
        users.Should().Contain(u => u.Login == "user2");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminUserService(context);

        var user = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash",
            Role = "User"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetByIdAsync(user.UserID, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Login.Should().Be("user1");
    }

    [Fact]
    public async Task SetRoleAsync_UpdatesUserRole()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminUserService(context);

        var user = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash",
            Role = "User"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        await service.SetRoleAsync(user.UserID, "Manager", CancellationToken.None);

        // Assert
        var updatedUser = await context.Users.FindAsync(user.UserID);
        updatedUser!.Role.Should().Be("Manager");
    }

    [Fact]
    public async Task SetRoleAsync_WithInvalidRole_ThrowsException()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminUserService(context);

        var user = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash",
            Role = "User"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var act = () => service.SetRoleAsync(user.UserID, "InvalidRole", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Invalid role*");
    }

    [Fact]
    public async Task SetActiveAsync_UpdatesUserActiveStatus()
    {
        // Arrange
        await using var context = TestContextFactory.CreateContext();
        var service = new AdminUserService(context);

        var user = new User
        {
            Login = "user1",
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hash",
            IsActive = true
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        await service.SetActiveAsync(user.UserID, false, CancellationToken.None);

        // Assert
        var updatedUser = await context.Users.FindAsync(user.UserID);
        updatedUser!.IsActive.Should().BeFalse();
    }
}

