using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class BulkUploadErrorListConfiguration : IEntityTypeConfiguration<BulkUploadErrorList>
{
    public void Configure(EntityTypeBuilder<BulkUploadErrorList> builder)
    {
        builder.ToTable("BulkUploadErrorList");
        builder.HasIndex(e => e.ProcessId, "IX_BulkUploadErrorList_ProcessId");
        builder.Property(e => e.RowId);
        builder.Property(e => e.ErrorField);
        builder.Property(e => e.ErrorMessage);
    }
}
