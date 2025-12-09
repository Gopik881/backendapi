using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class SubModuleMastersConfigurations : IEntityTypeConfiguration<SubModuleMaster>
{
    public void Configure(EntityTypeBuilder<SubModuleMaster> builder)
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
