using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Elixir.Infrastructure.Data.Configurations;

public class SubMenuItemsAccessMappingConfiguration : IEntityTypeConfiguration<SubMenuItemsAccessMapping>
{
    public void Configure(EntityTypeBuilder<SubMenuItemsAccessMapping> builder)
    {
        builder.ToTable("SubMenuItemsAccessMapping");



    }
}
