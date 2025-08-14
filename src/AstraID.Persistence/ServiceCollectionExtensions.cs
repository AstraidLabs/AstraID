using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AstraID.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["ASTRAID_DB_PROVIDER"] ?? "SqlServer";
        var conn = configuration["ASTRAID_DB_CONN"] ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstraID;";
        services.AddDbContext<AstraIdDbContext>(options =>
        {
            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlServer(conn);
            }
        });
        services.AddDataProtection().PersistKeysToDbContext<AstraIdDbContext>();
        return services;
    }
}
