using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class TelephoneCodeMasterConfiguration : IEntityTypeConfiguration<TelephoneCodeMaster>
{
    public void Configure(EntityTypeBuilder<TelephoneCodeMaster> builder)
    {
        builder.ToTable("TelephoneCodeMaster");

        builder.HasIndex(e => e.CountryId, "IX_TelephoneCodeMaster_CountryId");

        builder.HasIndex(e => new { e.CountryId, e.TelephoneCode }, "UQ_TelephoneCodeMaster_CountryId_TelephoneCode").IsUnique();

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Description).IsUnicode(false);
        builder.Property(e => e.TelephoneCode)
            .HasMaxLength(5)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
