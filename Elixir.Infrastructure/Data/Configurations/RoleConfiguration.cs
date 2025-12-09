using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .IsUnicode(false);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.RoleName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

    }
}
