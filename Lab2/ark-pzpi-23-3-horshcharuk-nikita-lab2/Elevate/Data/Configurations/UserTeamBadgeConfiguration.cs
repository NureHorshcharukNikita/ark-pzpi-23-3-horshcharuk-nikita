using Elevate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elevate.Data.Configurations;

public class UserTeamBadgeConfiguration : IEntityTypeConfiguration<UserTeamBadge>
{
    public void Configure(EntityTypeBuilder<UserTeamBadge> builder)
    {
        builder.HasOne(utb => utb.User)
            .WithMany(u => u.UserTeamBadges)
            .HasForeignKey(utb => utb.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(utb => utb.TeamBadge)
            .WithMany(tb => tb.UserTeamBadges)
            .HasForeignKey(utb => utb.TeamBadgeID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(utb => utb.Team)
            .WithMany(t => t.UserTeamBadges)
            .HasForeignKey(utb => utb.TeamID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(utb => new { utb.UserID, utb.TeamBadgeID, utb.TeamID })
            .IsUnique();
    }
}
