using AstraID.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.DisplayNameRaw)
            .HasMaxLength(128)
            .HasColumnType("nvarchar(128)");

        builder.Property(u => u.IsActive)
            .HasColumnType("bit");

        builder.Property(u => u.CreatedUtc)
            .HasColumnType("datetime2");

        builder.Property(u => u.LastLoginUtc)
            .HasColumnType("datetime2");

        builder.Property(u => u.TenantId)
            .HasColumnType("uniqueidentifier");

        builder.HasIndex(u => u.NormalizedEmail);
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.TenantId);
    }
}
