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
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .Build();

        var conn = configuration.GetConnectionString("Default") ?? configuration["ConnectionStrings:Default"];
        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("ConnectionStrings:Default is not configured in appsettings*.json.");

        var options = new DbContextOptionsBuilder<AstraIdDbContext>();
        options.UseSqlServer(conn);

        return new AstraIdDbContext(options.Options);
    }
}
