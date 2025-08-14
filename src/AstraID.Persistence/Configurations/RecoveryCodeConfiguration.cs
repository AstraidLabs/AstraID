using AstraID.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class RecoveryCodeConfiguration : IEntityTypeConfiguration<RecoveryCode>
{
    public void Configure(EntityTypeBuilder<RecoveryCode> builder)
    {
        builder.ToTable("RecoveryCodes");

        builder.Property(r => r.UserId).HasColumnType("uniqueidentifier");
        builder.Property(r => r.CodeHash)
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");
        builder.Property(r => r.CreatedUtc).HasColumnType("datetime2");
        builder.Property(r => r.UsedUtc).HasColumnType("datetime2");

        builder.HasIndex(r => new { r.UserId, r.CodeHash }).IsUnique();
    }
}
