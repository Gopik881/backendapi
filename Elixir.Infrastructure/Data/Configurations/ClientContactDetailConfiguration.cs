using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class ClientContactDetailConfiguration : IEntityTypeConfiguration<ClientContactDetail>
{
    public void Configure(EntityTypeBuilder<ClientContactDetail> builder)
    {
        builder.HasIndex(e => e.ClientId, "IX_ClientContactDetails_ClientId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Department)
            .HasMaxLength(45)
            .IsUnicode(false);
        builder.Property(e => e.Designation)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.FirstName)
            .HasMaxLength(45)
            .IsUnicode(false);
        builder.Property(e => e.LastName)
            .HasMaxLength(45)
            .IsUnicode(false);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.Remarks).IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
