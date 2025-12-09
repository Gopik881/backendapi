using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class CompanyAdminUsersHistoryConfiguration : IEntityTypeConfiguration<CompanyAdminUsersHistory>
{
    public void Configure(EntityTypeBuilder<CompanyAdminUsersHistory> builder)
    {
        builder.ToTable("CompanyAdminUsersHistory");

        builder.HasIndex(e => e.CompanyId, "IX_CompanyAdminUsersHistory_CompanyId");

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
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        builder.Property(e => e.Version).HasDefaultValue(1);



    }
}
