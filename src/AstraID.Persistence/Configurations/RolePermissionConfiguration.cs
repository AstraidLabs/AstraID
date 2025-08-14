using AstraID.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.Property(rp => rp.RoleId).HasColumnType("uniqueidentifier");
        builder.Property(rp => rp.PermissionId).HasColumnType("int");

        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();
    }
}
