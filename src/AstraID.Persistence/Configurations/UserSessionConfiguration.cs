using AstraID.Domain.Entities;
using AstraID.Domain.ValueObjects;
using AstraID.Persistence.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");

        builder.Property(s => s.UserId).HasColumnType("uniqueidentifier");
        builder.Property(s => s.TenantId).HasColumnType("uniqueidentifier");

        builder.Property(s => s.Device)
            .HasMaxLength(128)
            .HasColumnType("nvarchar(128)");

        builder.Property(s => s.DeviceId)
            .HasConversion(d => d.Value, v => DeviceId.Create(v))
            .HasMaxLength(128)
            .HasColumnType("nvarchar(128)");

        builder.Property(s => s.Ip)
            .HasConversion(ValueObjectConverters.IpAddressConverter, ValueObjectConverters.IpAddressComparer)
            .HasColumnName("Ip")
            .HasMaxLength(45)
            .HasColumnType("nvarchar(45)");

        builder.Property(s => s.Agent)
            .HasConversion(ValueObjectConverters.UserAgentConverter, ValueObjectConverters.UserAgentComparer)
            .HasColumnName("Agent")
            .HasMaxLength(512)
            .HasColumnType("nvarchar(512)");

        builder.Property(s => s.CreatedUtc).HasColumnType("datetime2");
        builder.Property(s => s.LastSeenUtc).HasColumnType("datetime2");
        builder.Property(s => s.RevokedUtc).HasColumnType("datetime2");

        builder.Property(s => s.RevokeReason)
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");

        builder.HasIndex(s => new { s.UserId, s.CreatedUtc });

        builder.HasIndex(s => new { s.UserId, s.DeviceId, s.RevokedUtc })
            .IsUnique()
            .HasFilter("[RevokedUtc] IS NULL");
    }
}
