using Elixir.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elixir.Infrastructure.Data.Configurations;

public class SuperUserConfiguration : IEntityTypeConfiguration<SuperUser>
{
    public void Configure(EntityTypeBuilder<SuperUser> builder)
    {
        builder.HasIndex(e => e.EmailHash, "IX_SuperUsers_EmailHash");

        builder.HasIndex(e => e.Email, "UQ_SuperUsers_Email").IsUnique();

        builder.Property(e => e.Designation)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.LastName)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.Location)
            .HasMaxLength(100)
            .IsUnicode(false);
        builder.Property(e => e.PasswordHash)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15)
            .IsUnicode(false);
        builder.Property(e => e.ProfilePicture).IsUnicode(false);
        builder.Property(e => e.Salt)
            .HasMaxLength(64)
            .IsUnicode(false);
        builder.Property(e => e.LastSessionActiveUntil).HasColumnType("datetime");


    }
}
