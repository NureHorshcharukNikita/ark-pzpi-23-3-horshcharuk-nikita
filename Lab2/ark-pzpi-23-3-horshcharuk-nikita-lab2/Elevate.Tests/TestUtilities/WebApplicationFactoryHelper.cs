using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Auth;
using Elevate.Services.Auth.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Text;

namespace Elevate.Tests.TestUtilities;

public class CustomWebApplicationFactory : WebApplicationFactory<Elevate.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            var testSettings = new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "Elevate.Tests" },
                { "Jwt:Audience", "Elevate.Tests" },
                { "Jwt:SigningKey", "UnitTestSigningKey_ShouldBeLongEnough_12345678901234567890" },
                { "Jwt:ExpiresMinutes", "60" }
            };
            config.AddInMemoryCollection(testSettings);
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ElevateDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ElevateDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_Shared");
            });

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var testSigningKey = "UnitTestSigningKey_ShouldBeLongEnough_12345678901234567890";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "Elevate.Tests",
                    ValidAudience = "Elevate.Tests",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(testSigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        });

        builder.UseEnvironment("Development");
    }
}

public static class AuthenticationHelper
{
    public static async Task<string> GenerateTokenAsync(
        ElevateDbContext context,
        User user,
        IJwtTokenService tokenService
    )
    {
        var tokenResult = tokenService.CreateToken(user);
        return tokenResult.Token;
    }

    public static async Task<User> CreateTestUserAsync(
        ElevateDbContext context,
        IPasswordHasher<User> passwordHasher,
        string login = "testuser",
        string password = "TestPassword123!",
        string role = "User")
    {
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
        return user;
    }
}

