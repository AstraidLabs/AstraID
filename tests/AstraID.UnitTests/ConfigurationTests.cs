using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AstraID.Api.Options;
using Xunit;

namespace AstraID.UnitTests;

public class ConfigurationTests
{
    [Fact]
    public void ConnectionString_Comes_From_File_Only()
    {
        var json = "{\"ConnectionStrings\":{\"Default\":\"DataSource=:memory:\"}}";
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, json);
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", "EnvValue");
        var cfg = new ConfigurationBuilder()
            .AddJsonFile(tmp, optional: false)
            .Build();
        cfg.GetConnectionString("Default").Should().Be("DataSource=:memory:");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", null);
        File.Delete(tmp);
    }

    [Fact]
    public void Missing_ConnectionString_Fails_Validation()
    {
        var json = "{ }";
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, json);
        var cfg = new ConfigurationBuilder().AddJsonFile(tmp).Build();
        var services = new ServiceCollection();
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(cfg.GetSection("ConnectionStrings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<ConnectionStringsOptions>, ConnectionStringsOptionsValidator>();
        var sp = services.BuildServiceProvider();
        Action act = () => _ = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;
        act.Should().Throw<OptionsValidationException>();
    }

    [Fact]
    public void Introspection_Mode_Requires_Credentials()
    {
        var json = "{\"Auth\":{\"ValidationMode\":\"Introspection\",\"Certificates\":{\"UseDevelopmentCertificates\":true}}}";
        var tmp = Path.GetTempFileName();
        File.WriteAllText(tmp, json);
        var cfg = new ConfigurationBuilder().AddJsonFile(tmp).Build();
        var services = new ServiceCollection();
        services.AddOptions<AuthOptions>()
            .Bind(cfg.GetSection("Auth"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<AuthOptions>, AuthOptionsValidator>();
        var sp = services.BuildServiceProvider();
        Action act = () => _ = sp.GetRequiredService<IOptions<AuthOptions>>().Value;
        act.Should().Throw<OptionsValidationException>();

        json = "{\"Auth\":{\"ValidationMode\":\"Introspection\",\"Certificates\":{\"UseDevelopmentCertificates\":true},\"Introspection\":{\"ClientId\":\"id\",\"ClientSecret\":\"secret\"}}}";
        File.WriteAllText(tmp, json);
        cfg = new ConfigurationBuilder().AddJsonFile(tmp).Build();
        services = new ServiceCollection();
        services.AddOptions<AuthOptions>()
            .Bind(cfg.GetSection("Auth"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<AuthOptions>, AuthOptionsValidator>();
        sp = services.BuildServiceProvider();
        act = () => _ = sp.GetRequiredService<IOptions<AuthOptions>>().Value;
        act.Should().NotThrow();
    }
}
