using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AstraID.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["ASTRAID_DB_PROVIDER"]?.ToLowerInvariant() ?? "sqlserver";
        var conn = configuration["ASTRAID_DB_CONN"] ?? throw new InvalidOperationException("Connection string missing");

        services.AddDbContext<AstraIdDbContext>(opt =>
        {
            if (provider == "postgres")
                opt.UseNpgsql(conn);
            else
                opt.UseSqlServer(conn);
        });

        services.AddDataProtection().PersistKeysToDbContext<AstraIdDbContext>();
        return services;
    }
}
