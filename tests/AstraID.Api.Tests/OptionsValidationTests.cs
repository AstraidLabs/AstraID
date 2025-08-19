using System;
using System.IO;
using AstraID.Api.Options;
using AstraID.Api.Infrastructure.JsoncConfiguration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Xunit;

class FakeHostEnvironment : IHostEnvironment
{
    public FakeHostEnvironment(string env)
    {
        EnvironmentName = env;
        ApplicationName = "Test";
        ContentRootPath = ".";
    }
    public string EnvironmentName { get; set; }
    public string ApplicationName { get; set; }
    public string ContentRootPath { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}

public class AstraIdOptionsValidatorTests
{
    [Fact]
    public void InvalidIssuerFails()
    {
        var options = new AstraIdOptions { Issuer = "http://localhost", AllowedCors = Array.Empty<string>(), RateLimit = new RateLimitOptions { Rps = 1, Burst = 1 } };
        var validator = new AstraIdOptionsValidator(new FakeHostEnvironment("Production"));
        var result = validator.Validate(null, options);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public void HttpCorsNotAllowedInProduction()
    {
        var options = new AstraIdOptions { Issuer = "https://example.com", AllowedCors = new[] { "http://example.com" }, RateLimit = new RateLimitOptions { Rps = 1, Burst = 1 } };
        var validator = new AstraIdOptionsValidator(new FakeHostEnvironment("Production"));
        var result = validator.Validate(null, options);
        Assert.False(result.Succeeded);
    }
}

public class ConnectionStringsOptionsValidatorTests
{
    [Fact]
    public void EmptyConnectionStringFails()
    {
        var options = new ConnectionStringsOptions { Default = string.Empty };
        var validator = new ConnectionStringsOptionsValidator();
        var result = validator.Validate(null, options);
        Assert.False(result.Succeeded);
    }
}

public class AuthOptionsValidatorTests
{
    [Fact]
    public void MissingIntrospectionSecretFails()
    {
        var options = new AuthOptions
        {
            ValidationMode = ValidationMode.Introspection,
            Introspection = new IntrospectionOptions { ClientId = "id", ClientSecret = "" },
            Certificates = new CertificateCollectionOptions { UseDevelopmentCertificates = true }
        };
        var validator = new AuthOptionsValidator();
        var result = validator.Validate(null, options);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public void MissingCertificateFileFails()
    {
        var options = new AuthOptions
        {
            Certificates = new CertificateCollectionOptions
            {
                UseDevelopmentCertificates = false,
                Signing = new[] { new CertificateOptions { Path = "nonexistent.pfx" } },
                Encryption = Array.Empty<CertificateOptions>()
            }
        };
        var validator = new AuthOptionsValidator();
        var result = validator.Validate(null, options);
        Assert.False(result.Succeeded);
    }
}

public class JsoncConfigurationProviderTests
{
    [Fact]
    public void CommentsAreIgnored()
    {
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, "{\n//c1\n\"A\":1/*b*/\n}");
        var config = new ConfigurationBuilder().AddJsoncFile(temp, optional: false, reloadOnChange: false).Build();
        Assert.Equal("1", config["A"]);
        File.Delete(temp);
    }
}
