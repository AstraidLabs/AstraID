using AstraID.Domain.Abstractions;
using AstraID.Domain.Repositories;
using AstraID.Infrastructure.Messaging;
using AstraID.Infrastructure.Messaging.Background;
using AstraID.Infrastructure.Persistence;
using AstraID.Infrastructure.Persistence.Interceptors;
using AstraID.Infrastructure.Persistence.Repositories;
using AstraID.Infrastructure.OpenIddict;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            var provider = configuration["ASTRAID_DB_PROVIDER"] ?? "SqlServer";
            var conn = configuration["ASTRAID_DB_CONN"] ?? throw new InvalidOperationException("ASTRAID_DB_CONN is not configured.");

            if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
                opt.UseNpgsql(conn);
            else
                opt.UseSqlServer(conn);

            opt.AddInterceptors(interceptor);
        });

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

        return services;
    }
}
