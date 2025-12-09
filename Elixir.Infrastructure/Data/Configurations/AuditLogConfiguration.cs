using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog");

        builder.HasIndex(e => e.CompanyId, "IX_AuditLog_CompanyId");

        builder.Property(e => e.Action)
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.Details).IsUnicode(false);
        builder.Property(e => e.EntityName)
            .HasMaxLength(255)
            .IsUnicode(false);



    }
}
