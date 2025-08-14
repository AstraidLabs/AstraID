using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Infrastructure.Messaging;

/// <summary>
/// EF Core configuration for <see cref="OutboxMessage"/>.
/// </summary>
public class OutboxDbConfig : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Type).IsRequired().HasMaxLength(512);
        builder.Property(o => o.PayloadJson).IsRequired();
        builder.Property(o => o.CreatedUtc).IsRequired();
    }
}
