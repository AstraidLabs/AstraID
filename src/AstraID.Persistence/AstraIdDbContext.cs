using AstraID.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DataProtectionKeyEntity = Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey;

namespace AstraID.Persistence;

public class AstraIdDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IDataProtectionKeyContext
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
    public DbSet<DataProtectionKeyEntity> DataProtectionKeys => Set<DataProtectionKeyEntity>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("auth");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AstraIdDbContext).Assembly);
        modelBuilder.UseOpenIddict();
    }
}
