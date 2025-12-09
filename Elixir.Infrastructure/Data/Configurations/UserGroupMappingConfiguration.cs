using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class UserGroupMappingConfiguration : IEntityTypeConfiguration<UserGroupMapping>
{
    public void Configure(EntityTypeBuilder<UserGroupMapping> builder)
    {
        builder.HasIndex(e => e.UserGroupId, "IX_UserGroupMappings_UserGroupId");

        builder.HasIndex(e => new { e.UserId, e.UserGroupId }, "UQ_UserGroupMappings_UserId_UserGroupId").IsUnique();

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsEligible).HasDefaultValue(true);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
