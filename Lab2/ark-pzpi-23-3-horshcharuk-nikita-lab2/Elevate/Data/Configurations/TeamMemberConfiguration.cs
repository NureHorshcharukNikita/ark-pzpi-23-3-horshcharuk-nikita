using Elevate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elevate.Data.Configurations;

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasOne(tm => tm.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(tm => tm.TeamID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(tm => tm.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.TeamLevel)
            .WithMany(tl => tl.TeamMembers)
            .HasForeignKey(tm => tm.TeamLevelID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(tm => new { tm.TeamID, tm.UserID })
            .IsUnique();
    }
}
