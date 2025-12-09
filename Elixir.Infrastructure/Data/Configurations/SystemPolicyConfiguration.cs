using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class SystemPolicyConfiguration : IEntityTypeConfiguration<SystemPolicy>
{
    public void Configure(EntityTypeBuilder<SystemPolicy> builder)
    {

        builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        builder.Property(e => e.FileSizeLimitMb).HasDefaultValue(1);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.SessionTimeoutMinutes).HasDefaultValue(30);
        builder.Property(e => e.SpecialCharactersAllowed)
            .HasMaxLength(15)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
    }
}
