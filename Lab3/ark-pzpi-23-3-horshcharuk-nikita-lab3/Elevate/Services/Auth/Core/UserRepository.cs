using Elevate.Data;
using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Services.Auth.Core;

public class UserRepository
{
    private readonly ElevateDbContext _dbContext;

    public UserRepository(ElevateDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<User> FindByLoginOrEmailAsync(
        string loginOrEmail,
        CancellationToken cancellationToken)
    {
        var normalized = Normalize(loginOrEmail);

        var user = await _dbContext.Users
            .Include(u => u.TeamMemberships)
            .FirstOrDefaultAsync(
                u => u.Login.ToLower() == normalized ||
                     u.Email.ToLower() == normalized,
                cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return user;
    }

    public async Task<bool> ExistsByLoginOrEmailAsync(
        string login,
        string email,
        CancellationToken cancellationToken)
    {
        var normalizedLogin = Normalize(login);
        var normalizedEmail = Normalize(email);

        return await _dbContext.Users.AnyAsync(
            u => u.Login.ToLower() == normalizedLogin ||
                 u.Email.ToLower() == normalizedEmail,
            cancellationToken);
    }

    public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateLastLoginAsync(User user, CancellationToken cancellationToken)
    {
        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string Normalize(string value) =>
        value.Trim().ToLowerInvariant();
}

