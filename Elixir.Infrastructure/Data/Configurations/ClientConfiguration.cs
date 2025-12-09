using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasIndex(e => e.Id, "IX_Clients_ClientId");

        builder.Property(e => e.Address1)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.Address2)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.ClientCode)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.ClientInfo)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.ClientName)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(12)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        builder.Property(e => e.ZipCode)
            .HasMaxLength(20)
            .IsUnicode(false);


    }
}
