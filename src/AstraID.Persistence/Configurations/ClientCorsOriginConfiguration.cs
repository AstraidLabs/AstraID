using AstraID.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class ClientCorsOriginConfiguration : IEntityTypeConfiguration<ClientCorsOrigin>
{
    public void Configure(EntityTypeBuilder<ClientCorsOrigin> builder)
    {
        builder.ToTable("ClientCorsOrigins");

        builder.Property(o => o.ClientId)
            .HasColumnType("uniqueidentifier");

        builder.Property(o => o.Origin)
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        builder.HasIndex(o => new { o.ClientId, o.Origin }).IsUnique();
    }
}
