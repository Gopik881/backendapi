using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Infrastructure.Data.Configurations;

public class ActiveSessionConfiguration : IEntityTypeConfiguration<ActiveSession>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ActiveSession> builder)
    {
        builder.ToTable("ActiveSession");
        builder.HasIndex(e => e.UserId, "IX_ActiveSession_UserId");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Token)
            .HasMaxLength(500)
            .IsUnicode(false);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime");
        builder.Property(e => e.ExpiresAt).HasColumnType("datetime");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
    }
}
