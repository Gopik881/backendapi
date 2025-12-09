using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class StateMasterConfiguration : IEntityTypeConfiguration<StateMaster>
{
    public void Configure(EntityTypeBuilder<StateMaster> builder)
    {
        builder.ToTable("StateMaster");

        builder.HasIndex(e => e.CountryId, "IX_StateMaster_CountryId");

        builder.HasIndex(e => new { e.CountryId, e.StateName, e.StateShortName }, "UQ_StateMaster_CountryId_StateName_StateShortName").IsUnique();

        builder.HasIndex(e => e.StateShortName, "UQ_StateMaster_StateShortName").IsUnique();

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Description).IsUnicode(false);
        builder.Property(e => e.IsDeleted).HasDefaultValue(true);
        builder.Property(e => e.StateName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.StateShortName)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
