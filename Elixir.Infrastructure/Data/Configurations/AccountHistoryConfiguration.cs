using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class AccountHistoryConfiguration : IEntityTypeConfiguration<AccountHistory>
{
    public void Configure(EntityTypeBuilder<AccountHistory> builder)
    {

        builder.ToTable("AccountHistory");

        builder.HasIndex(e => e.CompanyId, "IX_AccountHistory_CompanyId");

        builder.Property(e => e.ContractId)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.ContractName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.EndDate).HasColumnType("datetime");
        builder.Property(e => e.Gstin)
            .HasMaxLength(15)
            .IsUnicode(false);
        builder.Property(e => e.IsOpenEnded).HasDefaultValue(false);
        builder.Property(e => e.LastUpdatedOn)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.LicenseProcurement)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Pan)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.PerUserStorageMb)
            .HasDefaultValue(0.00m)
            .HasColumnType("decimal(10, 2)");
        builder.Property(e => e.RenewalReminderDate).HasColumnType("datetime");
        builder.Property(e => e.StartDate).HasColumnType("datetime");
        builder.Property(e => e.Tan)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        builder.Property(e => e.Version).HasDefaultValue(1);


    }
}
