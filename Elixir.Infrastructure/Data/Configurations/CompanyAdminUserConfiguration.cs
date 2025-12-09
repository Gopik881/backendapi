using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class CompanyAdminUserConfiguration : IEntityTypeConfiguration<CompanyAdminUser>
{
    public void Configure(EntityTypeBuilder<CompanyAdminUser> builder)
    {
        builder.HasIndex(e => e.CompanyId, "IX_CompanyAdminUsers_CompanyId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Designation)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.LastName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.PasswordHash)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.Salt)
            .HasMaxLength(64)
            .IsUnicode(false);
        builder.Property(e => e.EmailHash);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
