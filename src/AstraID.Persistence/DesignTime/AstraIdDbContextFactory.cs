using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AstraID.Persistence.DesignTime;

internal sealed class AstraIdDbContextFactory : IDesignTimeDbContextFactory<AstraIdDbContext>
{
    public AstraIdDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var provider = configuration["ASTRAID_DB_PROVIDER"]?.ToLowerInvariant() ?? "sqlserver";
        var conn = configuration["ASTRAID_DB_CONN"] ?? throw new InvalidOperationException("ASTRAID_DB_CONN missing.");

        var options = new DbContextOptionsBuilder<AstraIdDbContext>();
        if (provider == "postgres")
            options.UseNpgsql(conn);
        else
            options.UseSqlServer(conn);

        return new AstraIdDbContext(options.Options);
    }
}
