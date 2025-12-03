using Elevate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elevate.Data.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasOne(d => d.Team)
            .WithMany(t => t.Devices)
            .HasForeignKey(d => d.TeamID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.DeviceKey)
            .IsUnique();
    }
}
