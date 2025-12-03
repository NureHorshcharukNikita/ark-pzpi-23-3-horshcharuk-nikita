using Elevate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elevate.Data.Configurations;

public class ActionTypeConfiguration : IEntityTypeConfiguration<ActionType>
{
    public void Configure(EntityTypeBuilder<ActionType> builder)
    {
        builder.HasOne(at => at.Team)
            .WithMany(t => t.ActionTypes)
            .HasForeignKey(at => at.TeamID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(at => new { at.TeamID, at.Code })
            .IsUnique();
    }
}
