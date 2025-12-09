using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class UserNotificationsMappingConfiguration : IEntityTypeConfiguration<UserNotificationsMapping>
{
    public void Configure(EntityTypeBuilder<UserNotificationsMapping> builder)
    {
        builder.ToTable("UserNotificationsMapping");

        builder.HasIndex(e => e.UserId, "IX_UserNotificationsMapping_UserId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsRead).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

    }
}
