using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class CompanyOnboardingStatusConfiguration : IEntityTypeConfiguration<CompanyOnboardingStatus>
{
    public void Configure(EntityTypeBuilder<CompanyOnboardingStatus> builder)
    {
        builder.ToTable("CompanyOnboardingStatus");

        builder.HasIndex(e => e.ClientId, "IX_CompanyOnboardingStatus_ClientId");

        builder.HasIndex(e => e.CompanyId, "IX_CompanyOnboardingStatus_CompanyId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.OnboardingStatus)
            .HasMaxLength(10)
            .IsUnicode(false);
        builder.Property(e => e.RejectedReason).IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");



    }
}
