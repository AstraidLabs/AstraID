using AstraID.Domain.Entities;
using AstraID.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstraID.Persistence.Configurations;

internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasIndex(c => c.ClientId).IsUnique();

        builder.Property(c => c.ClientId)
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        builder.Property(c => c.DisplayName)
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        builder.Property(c => c.Type)
            .HasColumnType("int");

        builder.Property(c => c.ClientSecretHashRaw)
            .HasColumnName("ClientSecretHash")
            .HasMaxLength(512)
            .HasColumnType("nvarchar(512)");

        builder.Property(c => c.CreatedUtc)
            .HasColumnType("datetime2");

        builder.Property(c => c.CreatedBy)
            .HasColumnType("uniqueidentifier");

        builder.Property(c => c.TenantId)
            .HasColumnType("uniqueidentifier");

        // EF Core attempts to map the public navigation properties
        // `Scopes`, `RedirectUris` and `PostLogoutRedirectUris` automatically
        // which causes a model validation error when using the backing field
        // configuration. Explicitly ignoring the properties avoids the
        // duplicate mapping and lets the owned collection configurations
        // for `_scopes`, `_redirectUris` and `_postLogoutRedirectUris` take effect.
        builder.Ignore(c => c.Scopes);
        builder.Ignore(c => c.RedirectUris);
        builder.Ignore(c => c.PostLogoutRedirectUris);

        builder.OwnsMany<Scope>("_scopes", b =>
        {
            b.ToTable("ClientScopes");
            b.WithOwner().HasForeignKey("ClientId");
            b.Property(s => s.Value)
                .HasColumnName("Name")
                .HasMaxLength(128)
                .HasColumnType("nvarchar(128)");
            b.HasKey("ClientId", "Value");
        });

        builder.OwnsMany<RedirectUri>("_redirectUris", b =>
        {
            b.ToTable("ClientRedirectUris");
            b.WithOwner().HasForeignKey("ClientId");
            b.Property(r => r.Value)
                .HasColumnName("Uri")
                .HasMaxLength(2048)
                .HasColumnType("nvarchar(2048)");
            b.HasKey("ClientId", "Value");
        });

        builder.OwnsMany<RedirectUri>("_postLogoutRedirectUris", b =>
        {
            b.ToTable("ClientPostLogoutRedirectUris");
            b.WithOwner().HasForeignKey("ClientId");
            b.Property(r => r.Value)
                .HasColumnName("Uri")
                .HasMaxLength(2048)
                .HasColumnType("nvarchar(2048)");
            b.HasKey("ClientId", "Value");
        });

        builder.Navigation("_scopes").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation("_redirectUris").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation("_postLogoutRedirectUris").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(c => c.CorsOrigins).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(c => c.SecretHistory).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(c => c.CorsOrigins)
            .WithOne()
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.SecretHistory)
            .WithOne()
            .HasForeignKey(h => h.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
