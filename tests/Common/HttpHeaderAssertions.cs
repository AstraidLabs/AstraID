using System.Net.Http;
using FluentAssertions;

namespace AstraID.Tests.Common;

public static class HttpHeaderAssertions
{
    public static void ShouldContainSecurityHeaders(this HttpResponseMessage response)
    {
        response.Headers.Should().ContainKey("X-Content-Type-Options");
        response.Headers.Should().ContainKey("X-Frame-Options");
        response.Headers.Should().ContainKey("Referrer-Policy");
        response.Headers.Should().ContainKey("Content-Security-Policy");
    }
}
