using Elevate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elevate.Data.Configurations;

public class DeviceScanConfiguration : IEntityTypeConfiguration<DeviceScan>
{
    public void Configure(EntityTypeBuilder<DeviceScan> builder)
    {
        builder.HasOne(ds => ds.Device)
            .WithMany(d => d.DeviceScans)
            .HasForeignKey(ds => ds.DeviceID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ds => ds.Team)
            .WithMany(t => t.DeviceScans)
            .HasForeignKey(ds => ds.TeamID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ds => ds.User)
            .WithMany(u => u.DeviceScans)
            .HasForeignKey(ds => ds.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ds => new { ds.TeamID, ds.UserID });
    }
}
    