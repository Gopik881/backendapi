using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class ElixirUsersHistoryConfiguration : IEntityTypeConfiguration<ElixirUsersHistory>
{
    public void Configure(EntityTypeBuilder<ElixirUsersHistory> builder)
    {

        builder.ToTable("ElixirUsersHistory");

        builder.HasIndex(e => e.CompanyId, "IX_ElixirUsersHistory_CompanyId");

        builder.HasIndex(e => e.UserId, "IX_ElixirUsersHistory_UserId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");


    }
}
