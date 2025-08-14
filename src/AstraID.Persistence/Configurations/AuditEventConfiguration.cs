using AstraID.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("AuditEvents");

        builder.Property(a => a.TenantId).HasColumnType("uniqueidentifier");
        builder.Property(a => a.UserId).HasColumnType("uniqueidentifier");
        builder.Property(a => a.ClientId)
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        builder.Property(a => a.EventType).HasColumnType("int");
        builder.Property(a => a.CreatedUtc).HasColumnType("datetime2");
        builder.Property(a => a.DataJson).HasColumnType("nvarchar(max)");
        builder.Property(a => a.FailureReason)
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");
        builder.Property(a => a.Correlation)
            .HasMaxLength(64)
            .HasColumnType("nvarchar(64)");
        builder.Property(a => a.Ip)
            .HasMaxLength(45)
            .HasColumnType("nvarchar(45)");
        builder.Property(a => a.Agent)
            .HasMaxLength(512)
            .HasColumnType("nvarchar(512)");
        builder.Property(a => a.ResourceId)
            .HasMaxLength(128)
            .HasColumnType("nvarchar(128)");
        builder.Property(a => a.Severity)
            .HasMaxLength(16)
            .HasColumnType("nvarchar(16)")
            .HasDefaultValue("Info");

        builder.HasIndex(a => new { a.EventType, a.CreatedUtc })
            .IsDescending(false, true);
        builder.HasIndex(a => new { a.UserId, a.CreatedUtc })
            .IsDescending(false, true);
        builder.HasIndex(a => new { a.ClientId, a.CreatedUtc })
            .IsDescending(false, true);
        builder.HasIndex(a => new { a.Correlation, a.CreatedUtc })
            .IsDescending(false, true);
    }
}
