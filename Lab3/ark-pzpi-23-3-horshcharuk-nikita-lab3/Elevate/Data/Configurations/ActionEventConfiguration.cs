using Elevate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elevate.Data.Configurations;

public class ActionEventConfiguration : IEntityTypeConfiguration<ActionEvent>
{
    public void Configure(EntityTypeBuilder<ActionEvent> builder)
    {
        builder.HasOne(ae => ae.User)
            .WithMany(u => u.ActionEvents)
            .HasForeignKey(ae => ae.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ae => ae.Team)
            .WithMany(t => t.ActionEvents)
            .HasForeignKey(ae => ae.TeamID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ae => ae.ActionType)
            .WithMany(at => at.ActionEvents)
            .HasForeignKey(ae => ae.ActionTypeID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ae => ae.SourceUser)
            .WithMany()
            .HasForeignKey(ae => ae.SourceUserID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(ae => new { ae.TeamID, ae.UserID });
    }
}
