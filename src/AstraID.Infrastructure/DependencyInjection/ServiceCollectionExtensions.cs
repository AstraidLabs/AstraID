using AstraID.Domain.Abstractions;
using AstraID.Domain.Repositories;
using AstraID.Infrastructure.Messaging;
using AstraID.Infrastructure.Messaging.Background;
using AstraID.Infrastructure.Persistence;
using AstraID.Infrastructure.Persistence.Interceptors;
using AstraID.Infrastructure.Persistence.Repositories;
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
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventsCollectorInterceptor>();
        services.AddDbContext<AstraIdDbContext>((sp, opt) =>
        {
            var interceptor = sp.GetRequiredService<DomainEventsCollectorInterceptor>();
            var conn = configuration["ASTRAID_DB_CONN"];
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
        return services;
    }

    public static IServiceCollection AddOutbox(this IServiceCollection services)
    {
        services.AddScoped<IOutboxPublisher, OutboxPublisher>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddHostedService<OutboxHostedService>();
        return services;
    }
}
