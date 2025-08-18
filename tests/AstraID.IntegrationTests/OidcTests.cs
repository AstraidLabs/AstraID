using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AstraID.Api;
using AstraID.Api.Controllers;
using AstraID.Api.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Abstractions;
using Xunit;
using FluentAssertions;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Server;

namespace AstraID.IntegrationTests;

public class TestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, o => { });
        });
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(OpenIddictConstants.Claims.Subject, Guid.NewGuid().ToString()),
            new Claim("scope", "openid profile email roles astra.admin"),
            new Claim(ClaimTypes.Role, "admin")
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public class OidcTests : IClassFixture<TestFactory>
{
    private readonly HttpClient _client;
    private readonly TestFactory _factory;

    public OidcTests(TestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Discovery_Present()
    {
        var json = await _client.GetFromJsonAsync<Dictionary<string, object>>("/.well-known/openid-configuration");
        json!.Should().ContainKey("issuer");
    }

    [Fact]
    public async Task Jwks_Exposed()
    {
        var json = await _client.GetFromJsonAsync<Dictionary<string, object>>("/.well-known/jwks");
        json!.Should().NotBeNull();
    }

    [Fact]
    public async Task UserInfo_ReturnsSub()
    {
        var json = await _client.GetFromJsonAsync<Dictionary<string, object>>("/connect/userinfo");
        json!.Should().ContainKey("sub");
    }

    [Fact]
    public void TokenLifetimes_FromConfig()
    {
        var opt = _factory.Services.GetRequiredService<IOptionsMonitor<OpenIddictServerOptions>>().CurrentValue;
        opt.AccessTokenLifetime.Should().Be(TimeSpan.FromMinutes(60));
        opt.IdentityTokenLifetime.Should().Be(TimeSpan.FromMinutes(15));
        opt.RefreshTokenLifetime.Should().Be(TimeSpan.FromDays(14));
    }

    [Fact]
    public async Task Admin_CanCrudUsersAndClients()
    {
        var userResp = await _client.PostAsJsonAsync("/api/admin/users", new AstraID.Api.DTOs.Admin.CreateUserDto
        {
            Email = "test@example.com",
            Password = "Pass123!"
        });
        userResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var userLocation = userResp.Headers.Location;
        var getUser = await _client.GetAsync(userLocation);
        getUser.StatusCode.Should().Be(HttpStatusCode.OK);

        var clientResp = await _client.PostAsJsonAsync("/api/admin/clients", new AstraID.Api.DTOs.Admin.CreateClientDto
        {
            ClientId = "client1"
        });
        clientResp.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
