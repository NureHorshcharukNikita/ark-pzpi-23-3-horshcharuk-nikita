using Elevate.Data;
using Elevate.Entities;
using Elevate.Services.Actions;
using Elevate.Services.Admin;
using Elevate.Services.Analytics;
using Elevate.Services.Auth.Core;
using Elevate.Services.Auth.Tokens;
using Elevate.Services.Gamification;
using Elevate.Services.IoT;
using Elevate.Services.Leaderboard;
using Elevate.Services.Teams;
using Elevate.Services.Users;
using ActionEventValidator = Elevate.Services.Actions.ActionEventValidator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Elevate
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            var app = builder.Build();

            await SeedDatabaseAsync(app);
            ConfigureMiddleware(app);

            await app.RunAsync();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(JwtOptions.SectionName));

            ConfigureDatabase(builder);
            ConfigureApplicationServices(builder.Services);
            ConfigureAuthentication(builder);
        }

        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Використовується InMemory база даних (рядок підключення не вказаний).");
                Console.ResetColor();

                builder.Services.AddDbContext<ElevateDbContext>(options =>
                    options.UseInMemoryDatabase("ElevateDb"));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Використовується SQL Server база даних");
                Console.WriteLine("Рядок підключення:");
                Console.WriteLine(connectionString);
                Console.ResetColor();

                builder.Services.AddDbContext<ElevateDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }
        }

        private static void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddScoped<DataSeeder>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            
            services.AddScoped<UserRepository>();
            services.AddScoped<UserValidator>();
            services.AddScoped<IAuthService, AuthService>();
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<ILeaderboardService, LeaderboardService>();
            services.AddScoped<IGamificationService, GamificationService>();
            services.AddScoped<ActionEventValidator>();
            services.AddScoped<IActionEventService, ActionEventService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IIoTService, IoTService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IAdminTeamService, AdminTeamService>();
            services.AddScoped<IAdminDeviceService, AdminDeviceService>();
            services.AddScoped<ITeamLevelsAdminService, TeamLevelsAdminService>();
            services.AddScoped<ITeamBadgesAdminService, TeamBadgesAdminService>();
            services.AddScoped<IActionTypesAdminService, ActionTypesAdminService>();
            services.AddScoped<IAdminAuditService, AdminAuditService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            var jwtOptions = builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>();

            var signingKey = jwtOptions?.SigningKey
                             ?? throw new InvalidOperationException("JWT signing key is missing");

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            builder.Services.AddAuthorization();
        }

        private static async Task SeedDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.SeedAsync();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                // Вимкнено HTTPS redirect для розробки, щоб IoT пристрої могли підключатися по HTTP
                // app.UseHttpsRedirection();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
