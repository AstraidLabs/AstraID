using AstraID.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.Property(p => p.Name)
            .HasMaxLength(128)
            .HasColumnType("nvarchar(128)");

        builder.Property(p => p.Description)
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");

        builder.Property(p => p.IsBuiltIn)
            .HasColumnType("bit");

        builder.HasIndex(p => p.Name).IsUnique();
    }
}
