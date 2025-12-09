using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Elixir.Infrastructure.Data.Configurations;

public class ModuleMastersConfiguration : IEntityTypeConfiguration<ModuleMaster>
{
    public void Configure(EntityTypeBuilder<ModuleMaster> builder)
    {
        builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.MasterName)
                .HasMaxLength(255)
                .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        
    }
}
