using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Messaging;

/// <summary>
/// EF Core configuration for <see cref="OutboxMessage"/>.
/// </summary>
public class OutboxDbConfig : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("Outbox");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Type).IsRequired().HasMaxLength(512);
        builder.Property(o => o.PayloadJson).IsRequired();
        builder.Property(o => o.CreatedUtc).IsRequired();
        builder.HasIndex(o => o.ProcessedUtc);
        builder.HasIndex(o => o.CreatedUtc);
    }
}
