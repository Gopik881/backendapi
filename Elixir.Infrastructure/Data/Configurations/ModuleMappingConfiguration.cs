using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ModuleMappingConfiguration : IEntityTypeConfiguration<ModuleMapping>
{
    public void Configure(EntityTypeBuilder<ModuleMapping> builder)
    {
        builder.ToTable("ModuleMapping");

        builder.HasIndex(e => e.CompanyId, "IX_ModuleMapping_CompanyId");

        builder.HasIndex(e => e.ModuleId, "IX_ModuleMapping_ModuleId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.IsMandatory).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
