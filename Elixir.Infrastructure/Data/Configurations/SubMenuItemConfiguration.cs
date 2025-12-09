using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class SubMenuItemConfiguration : IEntityTypeConfiguration<SubMenuItem>
{
    public void Configure(EntityTypeBuilder<SubMenuItem> builder)
    {
        builder.Property(e => e.CreatedAt)
               .HasDefaultValueSql("(getdate())")
               .HasColumnType("datetime");
        builder.Property(e => e.Description).IsUnicode(false);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.SubMenuItemName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.SubMenuItemsUrl)
            .HasMaxLength(255)
            .IsUnicode(false);


    }
}
