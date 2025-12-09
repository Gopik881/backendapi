using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ClientReportingToolLimitConfiguration : IEntityTypeConfiguration<ClientReportingToolLimit>
{
    public void Configure(EntityTypeBuilder<ClientReportingToolLimit> builder)
    {
        builder.HasIndex(e => e.ClientId, "IX_ClientReportingToolLimits_ClientId");

        builder.Property(e => e.ClientCustomerReportCreators)
            .HasMaxLength(36)
            .IsUnicode(false);
        builder.Property(e => e.ClientDashboardLibrary)
            .HasMaxLength(36)
            .IsUnicode(false);
        builder.Property(e => e.ClientDashboardPersonalLibrary)
            .HasMaxLength(36)
            .IsUnicode(false);
        builder.Property(e => e.ClientReportingAdmins)
            .HasMaxLength(36)
            .IsUnicode(false);
        builder.Property(e => e.ClientSavedReportQueriesLibrary)
            .HasMaxLength(36)
            .IsUnicode(false);
        builder.Property(e => e.ClientSavedReportQueriesPerUser)
            .HasMaxLength(36)
            .IsUnicode(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
