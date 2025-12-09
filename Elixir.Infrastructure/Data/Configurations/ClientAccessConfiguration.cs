using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class ClientAccessConfiguration : IEntityTypeConfiguration<ClientAccess>
{
    public void Configure(EntityTypeBuilder<ClientAccess> builder)
    {
        builder.ToTable("ClientAccess");

        builder.HasIndex(e => e.ClientId, "IX_ClientAccess_ClientId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.EnableReportAccess).HasDefaultValue(false);
        builder.Property(e => e.EnableWebQuery).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
