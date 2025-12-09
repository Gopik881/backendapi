using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class EscalationContactsHistoryConfiguration : IEntityTypeConfiguration<EscalationContactsHistory>
{
    public void Configure(EntityTypeBuilder<EscalationContactsHistory> builder)
    {
        builder.ToTable("EscalationContactsHistory");

        builder.HasIndex(e => e.CompanyId, "IX_EscalationContactsHistory_CompanyId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Department)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.Designation)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.LastName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.Remarks).IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        builder.Property(e => e.Version).HasDefaultValue(1);



    }
}
