using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ModuleScreensMasterConfiguration : IEntityTypeConfiguration<ModuleScreensMaster>
{
    public void Configure(EntityTypeBuilder<ModuleScreensMaster> builder)
    {
        builder.ToTable("ModuleScreensMaster");

        builder.HasIndex(e => e.SubModuleId, "IX_ModuleScreensMaster_SubModuleId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsModuleMasterType).HasDefaultValue(true);
        builder.Property(e => e.ModuleMasterName)
            .HasMaxLength(45)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
