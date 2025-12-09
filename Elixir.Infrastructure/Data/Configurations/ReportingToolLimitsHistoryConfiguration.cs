using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ReportingToolLimitsHistoryConfiguration : IEntityTypeConfiguration<ReportingToolLimitsHistory>
{
    public void Configure(EntityTypeBuilder<ReportingToolLimitsHistory> builder)
    {
        builder.ToTable("ReportingToolLimitsHistory");

        builder.HasIndex(e => e.CompanyId, "IX_ReportingToolLimitsHistory_CompanyId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.DashboardsInLibrary).HasDefaultValue(0);
        builder.Property(e => e.DashboardsInPersonalLibrary).HasDefaultValue(0);
        builder.Property(e => e.LetterGenerationAdmins).HasDefaultValue(0);
        builder.Property(e => e.NoOfCustomReportCreators).HasDefaultValue(0);
        builder.Property(e => e.NoOfReportingAdmins).HasDefaultValue(0);
        builder.Property(e => e.SavedReportQueriesInLibrary).HasDefaultValue(0);
        builder.Property(e => e.SavedReportQueriesPerUser).HasDefaultValue(0);
        builder.Property(e => e.TemplatesSaved).HasDefaultValue(0);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        builder.Property(e => e.Version).HasDefaultValue(1);



    }
}
