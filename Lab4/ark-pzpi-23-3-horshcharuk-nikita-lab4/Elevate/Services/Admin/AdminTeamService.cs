using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Admin;

public class AdminTeamService : IAdminTeamService
{
    private readonly ElevateDbContext _dbContext;

    public AdminTeamService(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Team> CreateTeamAsync(string name, string? description, int? managerUserId, CancellationToken ct = default)
    {
        var team = new Team
        {
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync(ct);

        if (managerUserId.HasValue)
        {
            var member = new TeamMember
            {
                TeamID = team.TeamID,
                UserID = managerUserId.Value,
                TeamRole = "Lead",
                JoinedAt = DateTime.UtcNow,
                TeamPoints = 0
            };
            _dbContext.TeamMembers.Add(member);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == managerUserId.Value, ct);
            if (user != null && user.Role == "User")
            {
                user.Role = "Manager";
            }

            await _dbContext.SaveChangesAsync(ct);
        }

        return team;
    }

    public async Task UpdateTeamAsync(int teamId, string name, string? description, CancellationToken ct = default)
    {
        var team = await _dbContext.Teams.FirstOrDefaultAsync(t => t.TeamID == teamId, ct)
                   ?? throw new InvalidOperationException("Team not found");

        team.Name = name;
        team.Description = description;

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteTeamAsync(int teamId, CancellationToken ct = default)
    {
        var team = await _dbContext.Teams.FirstOrDefaultAsync(t => t.TeamID == teamId, ct)
                   ?? throw new InvalidOperationException("Team not found");

        _dbContext.Teams.Remove(team);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task AddMemberAsync(int teamId, int userId, string teamRole, CancellationToken ct = default)
    {
        var exists = await _dbContext.TeamMembers
            .AnyAsync(tm => tm.TeamID == teamId && tm.UserID == userId, ct);

        if (exists) return;

        var member = new TeamMember
        {
            TeamID = teamId,
            UserID = userId,
            TeamRole = teamRole,
            JoinedAt = DateTime.UtcNow,
            TeamPoints = 0
        };

        _dbContext.TeamMembers.Add(member);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task RemoveMemberAsync(int teamId, int userId, CancellationToken ct = default)
    {
        var member = await _dbContext.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamID == teamId && tm.UserID == userId, ct)
            ?? throw new InvalidOperationException("Team member not found");

        _dbContext.TeamMembers.Remove(member);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task ChangeMemberRoleAsync(int teamId, int userId, string teamRole, CancellationToken ct = default)
    {
        var member = await _dbContext.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamID == teamId && tm.UserID == userId, ct)
            ?? throw new InvalidOperationException("Team member not found");

        member.TeamRole = teamRole;
        await _dbContext.SaveChangesAsync(ct);
    }
}

