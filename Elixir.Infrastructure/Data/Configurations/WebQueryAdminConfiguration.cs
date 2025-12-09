using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class WebQueryAdminConfiguration : IEntityTypeConfiguration<WebQueryAdmin>
{
    public void Configure(EntityTypeBuilder<WebQueryAdmin> builder)
    {
        builder.ToTable("WebQueryAdmin");

        builder.HasIndex(e => e.UserGroupId, "IX_WebQueryAdmin_UserGroupId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsSelected).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
