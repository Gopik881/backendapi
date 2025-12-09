using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(e => e.CreatedAt)
                 .HasDefaultValueSql("(getdate())")
                 .HasColumnType("datetime");
        builder.Property(e => e.IsRead).HasDefaultValue(false);
        builder.Property(e => e.Message).IsUnicode(false);
        builder.Property(e => e.CreatedBy).HasDefaultValue(0);
        builder.Property(e => e.UpdatedBy).HasDefaultValue(0);
        builder.Property(e => e.IsActive);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        builder.Property(e => e.CompanyId).HasDefaultValue(0);
        builder.Property(e => e.UserId).HasDefaultValue(0);
        builder.Property(e => e.NotificationType)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Title)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

    }
}
