using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class CompanyHistoryConfiguration : IEntityTypeConfiguration<CompanyHistory>
{
    public void Configure(EntityTypeBuilder<CompanyHistory> builder)
    {
        builder.ToTable("CompanyHistory");

        builder.HasIndex(e => e.ClientId, "IX_CompanyHistory_ClientId");

        builder.Property(e => e.Address1)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.Address2)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.BillingAddress1)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.BillingAddress2)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.BillingAddressSameAsCompany).HasDefaultValue(false);
        builder.Property(e => e.BillingPhoneNumber)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.BillingZipCode)
            .HasMaxLength(20)
            .IsUnicode(false);
        builder.Property(e => e.ClientId).HasDefaultValue(1);
        builder.Property(e => e.CompanyCode)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.CompanyName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.CompanyStorageConsumedGb)
            .HasDefaultValue(0.00m)
            .HasColumnType("decimal(10, 2)");
        builder.Property(e => e.CompanyStorageTotalGb)
            .HasDefaultValue(0.00m)
            .HasColumnType("decimal(10, 2)");
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.IsUnderEdit).HasDefaultValue(false);
        builder.Property(e => e.LastUpdatedOn)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.MfaEmail).HasDefaultValue(false);
        builder.Property(e => e.MfaEnabled).HasDefaultValue(false);
        builder.Property(e => e.MfaSms).HasDefaultValue(false);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        builder.Property(e => e.Version).HasDefaultValue(1);
        builder.Property(e => e.ZipCode)
            .HasMaxLength(20)
            .IsUnicode(false);


    }
}
