using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ElixirUserConfiguration : IEntityTypeConfiguration<ElixirUser>
{
    public void Configure(EntityTypeBuilder<ElixirUser> builder)
    {
        builder.HasIndex(e => e.CompanyId, "IX_ElixirUsers_CompanyId");

        builder.HasIndex(e => e.UserId, "IX_ElixirUsers_UserId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
