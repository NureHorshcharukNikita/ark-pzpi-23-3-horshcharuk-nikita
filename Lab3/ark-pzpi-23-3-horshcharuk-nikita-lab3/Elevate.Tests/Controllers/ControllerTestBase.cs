using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Auth.Tokens;
using Elevate.Tests.TestUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http;

namespace Elevate.Tests.Controllers;

public abstract class ControllerTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected ControllerTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task<HttpClient> CreateAuthenticatedClientAsync(
        string login = "testuser",
        string password = "TestPassword123!",
        string role = "User")
    {
        int userId;

        await using (var setupScope = Factory.Services.CreateAsyncScope())
        {
            var context = setupScope.ServiceProvider.GetRequiredService<ElevateDbContext>();
            var passwordHasher = setupScope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var user = new User
            {
                Login = login,
                Email = $"{login}@test.com",
                FirstName = "Test",
                LastName = "User",
                Role = role,
                PasswordHash = passwordHasher.HashPassword(null!, password)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            userId = user.UserID;
        }

        await using var tokenScope = Factory.Services.CreateAsyncScope();
        var jwtOptions = tokenScope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>();

        var tokenContext = tokenScope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var userForToken = await tokenContext.Users.FindAsync(userId);
        if (userForToken == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found in database");
        }

        var tokenService = new JwtTokenService(jwtOptions);
        var tokenResult = tokenService.CreateToken(userForToken);
        var token = tokenResult.Token;

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    protected async Task<(User user, HttpClient client)> CreateAuthenticatedUserAndClientAsync(
        string login = "testuser",
        string password = "TestPassword123!",
        string role = "User"
        )
    {
        int userId;

        await using (var setupScope = Factory.Services.CreateAsyncScope())
        {
            var context = setupScope.ServiceProvider.GetRequiredService<ElevateDbContext>();
            var passwordHasher = setupScope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var user = new User
            {
                Login = login,
                Email = $"{login}@test.com",
                FirstName = "Test",
                LastName = "User",
                Role = role,
                PasswordHash = passwordHasher.HashPassword(null!, password)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            userId = user.UserID;
        }

        await using var tokenScope = Factory.Services.CreateAsyncScope();
        var jwtOptions = tokenScope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>();

        var tokenContext = tokenScope.ServiceProvider.GetRequiredService<ElevateDbContext>();

        var userForToken = await tokenContext.Users.FindAsync(userId);
        if (userForToken == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found in database");
        }

        var tokenService = new JwtTokenService(jwtOptions);
        var tokenResult = tokenService.CreateToken(userForToken);
        var token = tokenResult.Token;

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return (userForToken, client);
    }

    protected IServiceScope CreateScope()
    {
        return Factory.Services.CreateScope();
    }

    protected async Task<User> GetUserFromContextAsync(ElevateDbContext context, User user)
    {
        var existingUser = await context.Users.FindAsync(user.UserID);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID {user.UserID} not found in database");
        }
        return existingUser;
    }
}
