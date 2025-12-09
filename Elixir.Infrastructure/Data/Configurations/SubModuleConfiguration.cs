using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class SubModuleConfiguration : IEntityTypeConfiguration<SubModule>
{
    public void Configure(EntityTypeBuilder<SubModule> builder)
    {
        builder.HasIndex(e => e.ModuleId, "IX_SubModules_ModuleId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.SubModuleParentId);
        builder.Property(e => e.SubModuleName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
