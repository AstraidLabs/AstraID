using AstraID.Domain.Entities;
using AstraID.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class UserConsentConfiguration : IEntityTypeConfiguration<UserConsent>
{
    public void Configure(EntityTypeBuilder<UserConsent> builder)
    {
        builder.ToTable("UserConsents");

        builder.Property(c => c.UserId).HasColumnType("uniqueidentifier");
        builder.Property(c => c.ClientId)
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");
        builder.Property(c => c.TenantId).HasColumnType("uniqueidentifier");
        builder.Property(c => c.GrantedUtc).HasColumnType("datetime2");
        builder.Property(c => c.RevokedUtc).HasColumnType("datetime2");

        builder.HasIndex(c => new { c.UserId, c.ClientId });

        builder.OwnsMany<Scope>("_scopes", b =>
        {
            b.ToTable("UserConsentScopes");
            b.WithOwner().HasForeignKey("ConsentId");
            b.Property(s => s.Value)
                .HasColumnName("Name")
                .HasMaxLength(128)
                .HasColumnType("nvarchar(128)");
            b.HasKey("ConsentId", nameof(Scope.Value));
        });

        builder.Navigation("_scopes").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
