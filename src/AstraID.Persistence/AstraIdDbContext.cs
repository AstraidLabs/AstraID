// EF Core DbContext integrating ASP.NET Identity and AstraID domain entities.
// Key responsibilities: configure schema, expose DbSets, and integrate OpenIddict & outbox.
// Why: central unit of work for persistence; keeps mapping logic in one place.
// Gotchas: default schema set to 'auth', ensure migrations align.

using System.Reflection;
using AstraID.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AstraID.Persistence.Messaging;
using AstraID.Persistence.Internal;
using DataProtectionKey = AstraID.Domain.Entities.DataProtectionKey;

namespace AstraID.Persistence;

public sealed class AstraIdDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AstraIdDbContext(DbContextOptions<AstraIdDbContext> options) : base(options)
    {
    }

    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserConsent> UserConsents => Set<UserConsent>();
    public DbSet<RecoveryCode> RecoveryCodes => Set<RecoveryCode>();
    public DbSet<PasswordHistory> PasswordHistory => Set<PasswordHistory>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientSecretHistory> ClientSecretHistory => Set<ClientSecretHistory>();
    public DbSet<ClientCorsOrigin> ClientCorsOrigins => Set<ClientCorsOrigin>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("auth"); // Cohesive schema for identity tables.
        modelBuilder.UseOpenIddict(); // Registers OpenIddict entities.
        ModelBuilderConventions.Apply(modelBuilder); // Apply shared value object conventions.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // Ensure deterministic mappings.
    }
}
