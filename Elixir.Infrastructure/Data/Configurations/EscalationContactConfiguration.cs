using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class EscalationContactConfiguration : IEntityTypeConfiguration<EscalationContact>
{
    public void Configure(EntityTypeBuilder<EscalationContact> builder)
    {
        builder.HasIndex(e => e.CompanyId, "IX_EscalationContacts_CompanyId");

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



    }
}
