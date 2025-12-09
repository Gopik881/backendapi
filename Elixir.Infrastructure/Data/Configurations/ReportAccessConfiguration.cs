using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ReportAccessConfiguration : IEntityTypeConfiguration<ReportAccess>
{
    public void Configure(EntityTypeBuilder<ReportAccess> builder)
    {

        builder.ToTable("ReportAccess");

        builder.HasIndex(e => e.UserGroupId, "IX_ReportAccess_UserGroupId");

        builder.Property(e => e.CanDownload).HasDefaultValue(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsSelected).HasDefaultValue(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

    }
}
