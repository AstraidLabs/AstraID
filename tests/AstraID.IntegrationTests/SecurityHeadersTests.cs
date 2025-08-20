using System.Net;
using System.Threading.Tasks;
using AstraID.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AstraID.IntegrationTests;

public class SecurityHeadersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SecurityHeadersTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_HasSecurityHeaders()
    {
        var resp = await _client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.True(resp.Headers.Contains("X-Content-Type-Options"));
        Assert.True(resp.Headers.Contains("X-Frame-Options"));
        Assert.True(resp.Headers.Contains("Content-Security-Policy"));
    }
}
