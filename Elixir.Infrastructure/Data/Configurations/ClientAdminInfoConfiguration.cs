using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class ClientAdminInfoConfiguration : IEntityTypeConfiguration<ClientAdminInfo>
{
    public void Configure(EntityTypeBuilder<ClientAdminInfo> builder)
    {
        builder.ToTable("ClientAdminInfo");

        builder.HasIndex(e => e.ClientId, "IX_ClientAdminInfo_ClientId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Designation)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.Email)
            .HasMaxLength(45)
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
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
