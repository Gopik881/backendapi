using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class HorizontalConfiguration : IEntityTypeConfiguration<Horizontal>
{
    public void Configure(EntityTypeBuilder<Horizontal> builder)
    {
        builder.HasIndex(e => e.UserGroupId, "IX_Horizontals_UserGroupId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Description).IsUnicode(false);
        builder.Property(e => e.HorizontalName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
