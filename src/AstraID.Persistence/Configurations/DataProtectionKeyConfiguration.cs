using AstraID.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class DataProtectionKeyConfiguration : IEntityTypeConfiguration<DataProtectionKey>
{
    public void Configure(EntityTypeBuilder<DataProtectionKey> builder)
    {
        builder.ToTable("DataProtectionKeys");

        builder.Property(k => k.FriendlyName)
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        builder.Property(k => k.Xml)
            .HasColumnType("nvarchar(max)");
    }
}
