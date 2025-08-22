using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using AstraID.Tests.Common;
using FluentAssertions;

namespace AstraID.IntegrationTests;

public class OidcBasicTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;

    public OidcBasicTests(IntegrationTestFixture factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(Skip = "Requires DB configuration")]
    public async Task Discovery_ReturnsIssuer()
    {
        var json = await _client.GetFromJsonAsync<Dictionary<string, object>>("/.well-known/openid-configuration");
        json!.Should().ContainKey("issuer");
    }

    [Fact(Skip = "Requires DB configuration")]
    public async Task Jwks_ReturnsKeys()
    {
        var json = await _client.GetFromJsonAsync<Dictionary<string, object>>("/connect/jwks");
        json!.Should().ContainKey("keys");
    }

    [Fact(Skip = "Requires DB configuration")]
    public async Task Health_EndpointsContainSecurityHeaders()
    {
        var response = await _client.GetAsync("/health/live");
        response.ShouldContainSecurityHeaders();
    }
}
