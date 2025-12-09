using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class CurrencyMasterConfiguration : IEntityTypeConfiguration<CurrencyMaster>
{
    public void Configure(EntityTypeBuilder<CurrencyMaster> builder)
    {
        builder.ToTable("CurrencyMaster");

        builder.HasIndex(e => e.CountryId, "IX_CurrencyMaster_CountryId");

        builder.HasIndex(e => new { e.CountryId, e.CurrencyName, e.CurrencyShortName }, "UQ_CurrencyMaster_CountryId_CurrencyName_CurrencyShortName").IsUnique();

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.CurrencyName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.CurrencyShortName)
            .HasMaxLength(5)
            .IsUnicode(false);
        builder.Property(e => e.Description).IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

    }
}
