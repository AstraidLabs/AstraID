using AstraID.Domain.Entities;
using AstraID.Persistence.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class ClientSecretHistoryConfiguration : IEntityTypeConfiguration<ClientSecretHistory>
{
    public void Configure(EntityTypeBuilder<ClientSecretHistory> builder)
    {
        builder.ToTable("ClientSecretHistory");

        builder.Property(h => h.ClientId)
            .HasColumnType("uniqueidentifier");

        builder.Property(h => h.SecretHash)
            .HasConversion(ValueObjectConverters.HashedSecretConverter, ValueObjectConverters.HashedSecretComparer)
            .HasColumnName("SecretHash")
            .HasMaxLength(512)
            .HasColumnType("nvarchar(512)");

        builder.Property(h => h.CreatedUtc)
            .HasColumnType("datetime2");

        builder.Property(h => h.Active)
            .HasColumnType("bit");

        builder.HasIndex(h => new { h.ClientId, h.Active })
            .IsUnique()
            .HasFilter("[Active] = 1");
    }
}
