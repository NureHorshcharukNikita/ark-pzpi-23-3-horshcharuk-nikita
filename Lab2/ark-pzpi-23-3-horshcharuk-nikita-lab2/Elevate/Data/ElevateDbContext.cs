using Elevate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Data;

public class ElevateDbContext : DbContext
{
    public ElevateDbContext(DbContextOptions<ElevateDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<TeamLevel> TeamLevels => Set<TeamLevel>();
    public DbSet<TeamBadge> TeamBadges => Set<TeamBadge>();
    public DbSet<UserTeamBadge> UserTeamBadges => Set<UserTeamBadge>();
    public DbSet<ActionType> ActionTypes => Set<ActionType>();
    public DbSet<ActionEvent> ActionEvents => Set<ActionEvent>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceScan> DeviceScans => Set<DeviceScan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElevateDbContext).Assembly);
    }
}
