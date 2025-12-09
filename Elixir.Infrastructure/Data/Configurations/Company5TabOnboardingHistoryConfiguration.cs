using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class Company5TabOnboardingHistoryConfiguration : IEntityTypeConfiguration<Company5TabOnboardingHistory>
{
    public void Configure(EntityTypeBuilder<Company5TabOnboardingHistory> builder)
    {
        builder.ToTable("Company5TabOnboardingHistory");

        builder.HasIndex(e => e.CompanyId, "IX_Company5TabOnboardingHistory_CompanyId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.Reason).IsUnicode(false);
        builder.Property(e => e.Status)
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(NULL)");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
