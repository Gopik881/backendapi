using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.Property(e => e.CreatedAt)
               .HasDefaultValueSql("(getdate())")
               .HasColumnType("datetime");
        builder.Property(e => e.Description).IsUnicode(false);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.MenuItemName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.MenuItemsIcon)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.MenuItemsUrl)
            .HasMaxLength(255)
            .IsUnicode(false);

    }
}
