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

        var provider = configuration["ASTRAID_DB_PROVIDER"];
        var conn = configuration["ASTRAID_DB_CONN"]
                   ?? configuration.GetConnectionString("Default");

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

        var options = new DbContextOptionsBuilder<AstraIdDbContext>();
        if (provider == "postgres")
            options.UseNpgsql(conn);
        else if (provider == "sqlite")
            options.UseSqlite(conn);
        else
            options.UseSqlServer(conn);

        return new AstraIdDbContext(options.Options);
    }
}
