using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AstraID.Api;
using AstraID.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Abstractions;
using Xunit;

namespace AstraID.IntegrationTests;

public class ValidationFactory : WebApplicationFactory<Program>
{
    private readonly string _validationMode;
    public ValidationFactory(string mode)
    {
        _validationMode = mode;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Auth:ValidationMode", _validationMode);
        builder.UseSetting("ASTRAID_DB_CONN", "Server=(localdb)\\mssqllocaldb;Database=test;Trusted_Connection=True;");
        builder.UseSetting("ASTRAID_DB_PROVIDER", "SqlServer");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AstraIdDbContext>));
            services.AddDbContext<AstraIdDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        });
        builder.Configure(app =>
        {
            app.MapGet("/protected", [Authorize] () => Results.Ok()).RequireAuthorization();
        });
    }
}

public class OpenIddictValidationTests
{
    private static async Task SeedClientsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "client",
            ClientSecret = "secret",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
            }
        });
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "id",
            ClientSecret = "secret",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
        });
    }

    private static async Task<string> RequestTokenAsync(HttpClient client)
    {
        var resp = await client.PostAsync("/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = "client",
            ["client_secret"] = "secret"
        }));
        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return json!["access_token"];
    }

    [Fact]
    public async Task LocalValidation_ValidAndInvalidTokens()
    {
        await using var factory = new ValidationFactory("Local");
        await SeedClientsAsync(factory.Services);
        var client = factory.CreateClient();
        var token = await RequestTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var ok = await client.GetAsync("/protected");
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bad");
        var unauthorized = await client.GetAsync("/protected");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);
    }

    [Fact]
    public async Task IntrospectionValidation_ValidAndInvalidTokens()
    {
        await using var factory = new ValidationFactory("Introspection");
        await SeedClientsAsync(factory.Services);
        var client = factory.CreateClient();
        var token = await RequestTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var ok = await client.GetAsync("/protected");
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "bad");
        var unauthorized = await client.GetAsync("/protected");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);
    }
}
