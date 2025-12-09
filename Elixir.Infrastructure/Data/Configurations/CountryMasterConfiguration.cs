using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class CountryMasterConfiguration : IEntityTypeConfiguration<CountryMaster>
{
    public void Configure(EntityTypeBuilder<CountryMaster> builder)
    {
        builder.ToTable("CountryMaster");

        builder.HasIndex(e => e.CountryName, "UQ_CountryMaster_CountryName").IsUnique();

        builder.HasIndex(e => new { e.CountryName, e.CountryShortName }, "UQ_CountryMaster_CountryName_CountryShortName").IsUnique();

        builder.HasIndex(e => e.CountryShortName, "UQ_CountryMaster_CountryShortName").IsUnique();

        builder.Property(e => e.CountryName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.CountryShortName)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Description).IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

    }
}
