using AstraID.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.ToTable("PasswordHistory");

        builder.Property(p => p.UserId).HasColumnType("uniqueidentifier");
        builder.Property(p => p.PasswordHash)
            .HasMaxLength(512)
            .HasColumnType("nvarchar(512)");
        builder.Property(p => p.ChangedUtc).HasColumnType("datetime2");

        builder.HasIndex(p => new { p.UserId, p.ChangedUtc })
            .IsDescending(false, true);
    }
}
