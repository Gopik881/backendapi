using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.HasIndex(e => e.CreatedBy, "IX_UserGroups_CreatedBy");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Description).IsUnicode(false);
        builder.Property(e => e.GroupName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.GroupType)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

    }
}
