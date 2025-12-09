using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class MasterConfiguration : IEntityTypeConfiguration<Master>
{
    public void Configure(EntityTypeBuilder<Master> builder)
    {
        builder.ToTable("Master");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.MasterName)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.MasterScreenUrl)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
    }
}
