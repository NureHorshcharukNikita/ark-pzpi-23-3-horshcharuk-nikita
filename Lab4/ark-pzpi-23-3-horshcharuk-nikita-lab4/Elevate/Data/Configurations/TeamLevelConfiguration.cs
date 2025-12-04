using Elevate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elevate.Data.Configurations;

public class TeamLevelConfiguration : IEntityTypeConfiguration<TeamLevel>
{
    public void Configure(EntityTypeBuilder<TeamLevel> builder)
    {
        builder.HasOne(tl => tl.Team)
            .WithMany(t => t.Levels)
            .HasForeignKey(tl => tl.TeamID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tl => tl.TeamID);
    }
}
