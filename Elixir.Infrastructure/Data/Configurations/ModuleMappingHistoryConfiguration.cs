using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ModuleMappingHistoryConfiguration : IEntityTypeConfiguration<ModuleMappingHistory>
{
    public void Configure(EntityTypeBuilder<ModuleMappingHistory> builder)
    {
        builder.ToTable("ModuleMappingHistory");

        builder.HasIndex(e => e.CompanyId, "IX_ModuleMappingHistory_CompanyId");

        builder.HasIndex(e => e.ModuleId, "IX_ModuleMappingHistory_ModuleId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.IsMandatory).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
