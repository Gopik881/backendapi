using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class WebQueryHorizontalCheckboxItemConfiguration : IEntityTypeConfiguration<WebQueryHorizontalCheckboxItem>
{
    public void Configure(EntityTypeBuilder<WebQueryHorizontalCheckboxItem> builder)
    {
        builder.HasIndex(e => e.HorizontalId, "IX_WebQueryHorizontalCheckboxItems_HorizontalId");

        builder.Property(e => e.CheckboxItemName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsSelected).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
