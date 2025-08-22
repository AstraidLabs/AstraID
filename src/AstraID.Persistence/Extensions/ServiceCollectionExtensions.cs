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
            var provider = cfg["ASTRAID_DB_PROVIDER"];
            var conn = cfg["ASTRAID_DB_CONN"]
                       ?? cfg.GetConnectionString("Default");

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException(
                    "Database connection is not configured. Set ASTRAID_DB_CONN or ConnectionStrings:Default.");

            provider = provider?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(provider))
            {
                if (conn.Contains("datasource", StringComparison.OrdinalIgnoreCase) ||
                    conn.Contains("filename", StringComparison.OrdinalIgnoreCase))
                    provider = "sqlite";
                else
                    provider = "sqlserver";
            }

            if (provider == "postgres")
                opt.UseNpgsql(conn);
            else if (provider == "sqlite")
                opt.UseSqlite(conn);
            else
                opt.UseSqlServer(conn);
        });

        return services;
    }
}
