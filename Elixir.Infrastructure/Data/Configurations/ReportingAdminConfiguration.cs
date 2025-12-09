using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ReportingAdminConfiguration : IEntityTypeConfiguration<ReportingAdmin>
{
    public void Configure(EntityTypeBuilder<ReportingAdmin> builder)
    {
        builder.ToTable("ReportingAdmin");

        builder.HasIndex(e => e.UserGroupId, "IX_ReportingAdmin_UserGroupId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsSelected).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
