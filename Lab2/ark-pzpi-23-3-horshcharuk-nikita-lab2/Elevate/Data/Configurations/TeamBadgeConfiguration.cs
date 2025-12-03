using Elevate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elevate.Data.Configurations;

public class TeamBadgeConfiguration : IEntityTypeConfiguration<TeamBadge>
{
    public void Configure(EntityTypeBuilder<TeamBadge> builder)
    {
        builder.HasOne(tb => tb.Team)
            .WithMany(t => t.Badges)
            .HasForeignKey(tb => tb.TeamID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tb => new { tb.TeamID, tb.Code })
            .IsUnique();
    }
}
