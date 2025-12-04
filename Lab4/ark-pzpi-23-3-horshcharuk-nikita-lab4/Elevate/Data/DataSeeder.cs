using Elevate.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Data;

public class DataSeeder
{
    private readonly ElevateDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DataSeeder(ElevateDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDatabaseCreatedAsync(cancellationToken);

        if (await HasUsersAsync(cancellationToken))
            return;

        var users = await SeedUsersAsync(cancellationToken);
        var teams = await SeedTeamsAsync(cancellationToken);
        var levels = await SeedTeamLevelsAsync(teams, cancellationToken);

        await SeedTeamMembersAsync(teams, users, levels, cancellationToken);
        await SeedActionTypesAsync(teams, cancellationToken);
        await SeedBadgesAsync(teams, cancellationToken);
        await SeedDevicesAsync(teams, cancellationToken);
    }

    private async Task EnsureDatabaseCreatedAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }

    private Task<bool> HasUsersAsync(CancellationToken cancellationToken)
        => _dbContext.Users.AnyAsync(cancellationToken);

    private async Task<Dictionary<string, User>> SeedUsersAsync(CancellationToken cancellationToken)
    {
        var data = new[]
        {
            CreateUser("admin",   "admin@elevate.local",   "System", "Admin",   "Admin"),
            CreateUser("manager", "manager@elevate.local", "Marta",  "Manager", "Manager"),
            CreateUser("jdoe",    "john.doe@elevate.local","John",   "Doe",     "User"),
            CreateUser("asmith",  "anna.smith@elevate.local","Anna", "Smith",   "User")
        };

        _dbContext.Users.AddRange(data);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return data.ToDictionary(u => u.Login, u => u);
    }

    private User CreateUser(string login, string email, string firstName, string lastName, string role)
    {
        var user = new User
        {
            Login = login,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, "Password123!");
        return user;
    }

    private async Task<Dictionary<string, Team>> SeedTeamsAsync(CancellationToken cancellationToken)
    {
        var backend = new Team { Name = "Backend Team", Description = "API & integrations" };
        var mobile = new Team { Name = "Mobile Team", Description = "Flutter development" };

        _dbContext.Teams.AddRange(backend, mobile);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new Dictionary<string, Team>
        {
            ["backend"] = backend,
            ["mobile"] = mobile
        };
    }

    private async Task<Dictionary<string, List<TeamLevel>>> SeedTeamLevelsAsync(
        Dictionary<string, Team> teams,
        CancellationToken cancellationToken)
    {
        var backend = teams["backend"];
        var mobile = teams["mobile"];

        var backendLevels = new List<TeamLevel>
        {
            new() { TeamID = backend.TeamID, Name = "Rookie",  RequiredPoints = 0,   OrderIndex = 1 },
            new() { TeamID = backend.TeamID, Name = "Pro",     RequiredPoints = 200, OrderIndex = 2 },
            new() { TeamID = backend.TeamID, Name = "Legend",  RequiredPoints = 500, OrderIndex = 3 }
        };

        var mobileLevels = new List<TeamLevel>
        {
            new() { TeamID = mobile.TeamID, Name = "Starter",   RequiredPoints = 0,   OrderIndex = 1 },
            new() { TeamID = mobile.TeamID, Name = "Champion",  RequiredPoints = 300, OrderIndex = 2 }
        };

        _dbContext.TeamLevels.AddRange(backendLevels);
        _dbContext.TeamLevels.AddRange(mobileLevels);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new Dictionary<string, List<TeamLevel>>
        {
            ["backend"] = backendLevels,
            ["mobile"] = mobileLevels
        };
    }

    private async Task SeedTeamMembersAsync(
        Dictionary<string, Team> teams,
        Dictionary<string, User> users,
        Dictionary<string, List<TeamLevel>> levels,
        CancellationToken cancellationToken)
    {
        var backend = teams["backend"];
        var mobile = teams["mobile"];

        var bl = levels["backend"];
        var ml = levels["mobile"];

        var members = new[]
        {
            new TeamMember { TeamID = backend.TeamID, UserID = users["admin"].UserID,   TeamRole = "Lead",   TeamPoints = 400, TeamLevelID = bl[1].TeamLevelID },
            new TeamMember { TeamID = backend.TeamID, UserID = users["jdoe"].UserID,    TeamRole = "Member", TeamPoints = 250, TeamLevelID = bl[1].TeamLevelID },
            new TeamMember { TeamID = backend.TeamID, UserID = users["asmith"].UserID,  TeamRole = "Member", TeamPoints = 120, TeamLevelID = bl[0].TeamLevelID },

            new TeamMember { TeamID = mobile.TeamID,  UserID = users["manager"].UserID, TeamRole = "Lead",   TeamPoints = 320, TeamLevelID = ml[1].TeamLevelID },
            new TeamMember { TeamID = mobile.TeamID,  UserID = users["asmith"].UserID,  TeamRole = "Member", TeamPoints = 210, TeamLevelID = ml[1].TeamLevelID }
        };

        _dbContext.TeamMembers.AddRange(members);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedActionTypesAsync(Dictionary<string, Team> teams, CancellationToken cancellationToken)
    {
        var backend = teams["backend"];
        var mobile = teams["mobile"];

        var types = new[]
        {
            new ActionType { TeamID = backend.TeamID, Code = "DEPLOY",       Name = "Production Deployment", DefaultPoints = 50 },
            new ActionType { TeamID = backend.TeamID, Code = "CODE_REVIEW",  Name = "Code Review Hero",      DefaultPoints = 20 },

            new ActionType { TeamID = mobile.TeamID,  Code = "HOTFIX",       Name = "Critical Hotfix",       DefaultPoints = 40 }
        };

        _dbContext.ActionTypes.AddRange(types);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    private async Task SeedBadgesAsync(Dictionary<string, Team> teams, CancellationToken cancellationToken)
    {
        var backend = teams["backend"];
        var mobile = teams["mobile"];

        var badges = new[]
        {
            new TeamBadge { TeamID = backend.TeamID, Code = "SPRINT_HERO", Name = "Sprint Hero", ConditionType = "PointsReached", ConditionValue = 200 },
            new TeamBadge { TeamID = backend.TeamID, Code = "OPS_GURU",    Name = "Ops Guru",    ConditionType = "PointsReached", ConditionValue = 400 },

            new TeamBadge { TeamID = mobile.TeamID,  Code = "MOBILE_STAR", Name = "Mobile Star", ConditionType = "PointsReached", ConditionValue = 250 },
        };

        _dbContext.TeamBadges.AddRange(badges);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedDevicesAsync(Dictionary<string, Team> teams, CancellationToken cancellationToken)
    {
        var backend = teams["backend"];
        var mobile = teams["mobile"];

        var devices = new[]
        {
            new Device { Name = "Backend Wall Panel", TeamID = backend.TeamID, DeviceKey = "device-backend-001", Location = "Kyiv" },
            new Device { Name = "Mobile LED Strip",   TeamID = mobile.TeamID,  DeviceKey = "device-mobile-001",  Location = "Lviv" }
        };

        _dbContext.Devices.AddRange(devices);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

