using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AstraID.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<AstraIdDbContext>(opt =>
        {
            var provider = cfg["ASTRAID_DB_PROVIDER"]?.ToLowerInvariant() ?? "sqlserver";
            var conn = cfg["ASTRAID_DB_CONN"] ?? throw new InvalidOperationException("ASTRAID_DB_CONN missing.");
            if (provider == "postgres")
                opt.UseNpgsql(conn);
            else
                opt.UseSqlServer(conn);
        });

        return services;
    }
}
