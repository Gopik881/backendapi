using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations
{
    public class SubModuleScreensConfiguration : IEntityTypeConfiguration<SubModuleScreen>
    {
        public void Configure(EntityTypeBuilder<SubModuleScreen> builder)
        {
            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            builder.Property(e => e.IsEnabled).HasDefaultValue(true);
            builder.Property(e => e.ScreenName)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
            
        }
    }
}
