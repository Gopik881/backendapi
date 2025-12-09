using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class UserPasswordHistoryConfiguration : IEntityTypeConfiguration<UserPasswordHistory>
{
    public void Configure(EntityTypeBuilder<UserPasswordHistory> builder)
    {
        builder.ToTable("UserPasswordHistory");

        builder.HasIndex(e => e.UserId, "IX_UserPasswordHistory_UserId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.PasswordHash)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.Salt)
    .HasMaxLength(64)
    .IsUnicode(false);


    }
}
