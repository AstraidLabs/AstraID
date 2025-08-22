using AstraID.Domain.Abstractions;
using AstraID.Domain.Policies;
using AstraID.Domain.Repositories;
using AstraID.Domain.Services;
using AstraID.Infrastructure.Messaging;
using AstraID.Infrastructure.Messaging.Background;
using AstraID.Infrastructure.OpenIddict;
using AstraID.Infrastructure.Persistence;
using AstraID.Infrastructure.Persistence.Interceptors;
using AstraID.Infrastructure.Persistence.Repositories;
using AstraID.Infrastructure.Services;
using AstraID.Shared.Services;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AstraID.Infrastructure.DependencyInjection;

/// <summary>
/// DI helpers for Infrastructure layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAstraIdPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventsCollectorInterceptor>();

        services.AddDbContext<AstraIdDbContext>((sp, opt) =>
        {
            var interceptor = sp.GetRequiredService<DomainEventsCollectorInterceptor>();

            // Allow database configuration via environment variables or standard
            // connection string settings used in test projects ("ConnectionStrings:Default").
            var provider = configuration["ASTRAID_DB_PROVIDER"];
            var conn = configuration["ASTRAID_DB_CONN"]
                       ?? configuration.GetConnectionString("Default");

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException(
                    "Database connection is not configured. Set ASTRAID_DB_CONN or ConnectionStrings:Default.");

            provider = provider?.Trim();

            // If provider not explicitly supplied, attempt to infer from connection string.
            if (string.IsNullOrWhiteSpace(provider))
            {
                if (conn.Contains("DataSource", StringComparison.OrdinalIgnoreCase) ||
                    conn.Contains("Filename", StringComparison.OrdinalIgnoreCase))
                    provider = "Sqlite";
                else
                    provider = "SqlServer";
            }

            if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
                opt.UseNpgsql(conn);
            else if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
                opt.UseSqlite(conn);
            else
                opt.UseSqlServer(conn);

            opt.AddInterceptors(interceptor);
        });

        // Ensure base DbContext can be resolved by services requiring it directly.
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<AstraIdDbContext>());

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IUserConsentRepository, UserConsentRepository>();
        services.AddScoped<IAuditEventRepository, AuditEventRepository>();
        services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();

        services.AddScoped<IOutboxPublisher, OutboxPublisher>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddHostedService<OutboxHostedService>();

        services.AddScoped<IClientApplicationBridge, ClientApplicationBridge>();

        // Domain policies
        services.AddSingleton<LockoutPolicy>();
        services.AddSingleton<TwoFactorPolicy>();
        services.AddSingleton<ClientSecretPolicy>();
        services.AddSingleton<ConsentPolicy>();

        // Cross-layer services
        services.TryAddSingleton<AstraID.Application.Abstractions.IDateTimeProvider, SystemDateTimeProvider>();
        services.TryAddSingleton<AstraID.Domain.Abstractions.IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<IPasswordPolicy, DefaultPasswordPolicy>();
        services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();

        // Domain services
        services.AddScoped<UserDomainService>();
        services.AddScoped<SessionDomainService>();
        services.AddScoped<ConsentDomainService>();
        services.AddScoped<ClientDomainService>();

        return services;
    }
}
