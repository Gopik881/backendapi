using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class UserGroupMenuMappingConfiguration : IEntityTypeConfiguration<UserGroupMenuMapping>
{
    public void Configure(EntityTypeBuilder<UserGroupMenuMapping> builder)
    {
        builder.ToTable("UserGroupMenuMapping");


        builder.HasIndex(e => e.SubMenuItemId, "IX_UserGroupMenuMapping_SubMenuItemId");

        builder.HasIndex(e => e.UserGroupId, "IX_UserGroupMenuMapping_UserGroupId");

        builder.Property(e => e.ApproveAccess).HasDefaultValue(false);
        builder.Property(e => e.CreateAccess).HasDefaultValue(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.EditAccess).HasDefaultValue(false);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.IsAllCompanies).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        builder.Property(e => e.ViewOnlyAccess).HasDefaultValue(false);



    }
}
