using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.Property(e => e.CreatedAt)
              .HasDefaultValueSql("(getdate())")
              .HasColumnType("datetime");
        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .IsUnicode(false);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.ModuleName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.ModuleUrl)
            .HasMaxLength(500)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

    }
}
