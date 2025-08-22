using AstraID.Api;
using AstraID.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace AstraID.Tests.Common;

/// <summary>
/// Spins up the full API using an in-memory database for integration tests.
/// </summary>
public class IntegrationTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASTRAID_DB_CONN"] = "Server=(localdb)\\mssqllocaldb;Database=AstraIDTest;Trusted_Connection=True;"
            });
        }); // enable dev features like swagger off
        builder.ConfigureServices(services =>
        {
            // Replace real database with in-memory provider
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AstraIdDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContext<AstraIdDbContext>(o => o.UseInMemoryDatabase("TestDb"));

            // ensure database created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AstraIdDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
