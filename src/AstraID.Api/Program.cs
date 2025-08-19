using AstraID.Api.Extensions;
using AstraID.Api.Identity;
using AstraID.Api.OpenIddict;
using AstraID.Api.Health;
using AstraID.Application.Abstractions;
using AstraID.Application.DependencyInjection;
using AstraID.Infrastructure.DependencyInjection;
using AstraID.Infrastructure.Startup;
using Serilog;
using Hellang.Middleware.ProblemDetails;
using AstraID.Api.Infrastructure.Audit;
using Microsoft.AspNetCore.HttpOverrides;
using AstraID.Api.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var config = builder.Configuration;

#if DEBUG
builder.Configuration.AddUserSecrets<Program>(optional: true);
#endif

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddOptions<AstraIdOptions>()
    .Bind(config.GetSection("AstraId"))
    .ValidateDataAnnotations()
    .Validate(options => options.AllowedCors.All(origin => Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps),
        "CORS origins must be absolute HTTPS URLs")
    .Validate(options => Uri.IsWellFormedUriString(options.Issuer, UriKind.Absolute), "Invalid Issuer URL")
    .ValidateOnStart();

builder.Services.AddOptions<AuthOptions>()
    .Bind(config.GetSection("Auth"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddAstraIdApplication()
    .AddAstraIdPersistence(builder.Configuration)
    .AddAstraIdOpenIddict(builder.Configuration)
    .AddAstraIdSecurity(builder.Configuration)
    .AddAstraIdProblemDetails()
    .AddAstraIdHealthChecks(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "AstraID API",
            Version = "v1",
            Description = @"## Admin Setup
See docs/Admin-Config-Guide.md for configuration instructions.

Required keys:
- `ConnectionStrings:Default`
- `AstraId:Issuer`
- `Auth:Introspection:ClientId`
- `Auth:Introspection:ClientSecret`"
        });
        var scheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Id = "Bearer",
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
            }
        };
        c.AddSecurityDefinition("Bearer", scheme);
        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            { scheme, Array.Empty<string>() }
        });
    });
}

WebApplication app;
try
{
    app = builder.Build();
}
catch (OptionsValidationException ex)
{
    foreach (var failure in ex.Failures)
    {
        Console.Error.WriteLine($"Configuration error in '{ex.OptionsName}': {failure}");
    }
    if (builder.Environment.IsProduction())
    {
        Environment.ExitCode = 1;
        return;
    }
    throw;
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();
app.UseSecurityHeaders();
app.UseSerilogRequestLogging();
app.UseCors("cors");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseProblemDetails();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/_diag/config", (IConfiguration configuration) =>
    {
        var root = (IConfigurationRoot)configuration;
        var masked = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ConnectionStrings:Default",
            "Auth:Introspection:ClientSecret"
        };
        var snapshot = new Dictionary<string, object?>();
        foreach (var kvp in configuration.AsEnumerable())
        {
            if (string.IsNullOrEmpty(kvp.Value)) continue;
            var provider = root.Providers.First(p => p.TryGet(kvp.Key, out _));
            var value = masked.Contains(kvp.Key) ? "***" : kvp.Value;
            snapshot[kvp.Key] = new { value, source = provider.ToString() };
        }
        return Results.Json(snapshot);
    });

    app.MapGet("/_diag/config/validate", (IServiceProvider sp) =>
    {
        var issues = new List<object>();
        try { _ = sp.GetRequiredService<IOptions<AstraIdOptions>>().Value; }
        catch (OptionsValidationException ex)
        {
            foreach (var f in ex.Failures)
                issues.Add(new { Key = ex.OptionsName, Message = f, HowToFix = string.Empty });
        }
        try { _ = sp.GetRequiredService<IOptions<AuthOptions>>().Value; }
        catch (OptionsValidationException ex)
        {
            foreach (var f in ex.Failures)
                issues.Add(new { Key = ex.OptionsName, Message = f, HowToFix = string.Empty });
        }
        var status = issues.Count == 0 ? "OK" : "ERROR";
        return Results.Json(new { status, issues });
    });
}

app.MapControllers();
app.MapUserEndpoints();
app.MapClientEndpoints();
app.MapAstraIdHealthChecks();

await app.UseAstraIdDatabaseAsync();

app.Run();
