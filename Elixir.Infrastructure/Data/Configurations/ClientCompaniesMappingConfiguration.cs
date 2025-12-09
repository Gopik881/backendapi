using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class ClientCompaniesMappingConfiguration : IEntityTypeConfiguration<ClientCompaniesMapping>
{
    public void Configure(EntityTypeBuilder<ClientCompaniesMapping> builder)
    {
        builder.ToTable("ClientCompaniesMapping");

        builder.HasIndex(e => e.ClientId, "IX_ClientCompaniesMapping_ClientId");

        builder.HasIndex(e => e.CompanyId, "IX_ClientCompaniesMapping_CompanyId");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime");




    }
}
