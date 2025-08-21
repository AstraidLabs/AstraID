using AstraID.Api.Extensions;
using AstraID.Api.Identity;
using AstraID.Api.OpenIddict;
using AstraID.Api.Health;
using AstraID.Application.Abstractions;
using AstraID.Application.DependencyInjection;
using AstraID.Infrastructure.DependencyInjection;
using AstraID.Infrastructure.Startup;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using AstraID.Domain.Entities;
using CorrelationId;
using OpenTelemetry.Trace;
using Hellang.Middleware.ProblemDetails;
using AstraID.Api.Infrastructure.Audit;
using AstraID.Api.Infrastructure.JsoncConfiguration;
using Microsoft.AspNetCore.HttpOverrides;
using AstraID.Api.Options;
using Microsoft.Extensions.Options;
using CorrelationId.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var preConfig = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true)
    .Build();
var useJsonc = preConfig.GetValue<bool>("UseJsonc");

builder.Configuration.Sources.Clear();
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
if (useJsonc)
{
    builder.Configuration
        .AddJsoncFile("appsettings.jsonc", optional: true, reloadOnChange: true)
        .AddJsoncFile($"appsettings.{builder.Environment.EnvironmentName}.jsonc", optional: true, reloadOnChange: true)
        .AddJsoncFile("appsettings.Local.jsonc", optional: true, reloadOnChange: true);
}

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var config = builder.Configuration;

#if DEBUG
builder.Configuration.AddUserSecrets<Program>(optional: true);
#endif

builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId());

builder.Services.AddOptions<AstraIdOptions>()
    .Bind(config.GetSection("AstraId"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<AuthOptions>()
    .Bind(config.GetSection("Auth"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<ConnectionStringsOptions>()
    .Bind(config.GetSection("ConnectionStrings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions<AstraIdOptions>, AstraIdOptionsValidator>();
builder.Services.AddSingleton<IValidateOptions<AuthOptions>, AuthOptionsValidator>();
builder.Services.AddSingleton<IValidateOptions<ConnectionStringsOptions>, ConnectionStringsOptionsValidator>();

builder.Services.AddRouting();
builder.Services.AddCorrelationId();

builder.Services
    .AddAstraIdApplication()
    .AddAstraIdPersistence(builder.Configuration);

// Data Protection keys persisted in DB (for multi-instance and stable cookies)
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AstraID.Persistence.AstraIdDbContext>();

// ASP.NET Identity (core only, no UI)
builder.Services.AddIdentityCore<AppUser>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.SignIn.RequireConfirmedEmail = true;
    })
    .AddRoles<AppRole>()
    .AddEntityFrameworkStores<AstraID.Persistence.AstraIdDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(o =>
{
    o.Password.RequiredLength = 12;
    o.Password.RequireUppercase = true;
    o.Password.RequireLowercase = true;
    o.Password.RequireDigit = true;
    o.Password.RequireNonAlphanumeric = false;
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
});

builder.Services
    .AddAstraIdOpenIddict(builder.Configuration)
    .AddAstraIdSecurity(builder.Configuration)
    .AddAstraIdProblemDetails()
    .AddAstraIdHealthChecks(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation()
        .AddOtlpExporter());

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
        var (key, message, fix) = ValidationError.Parse(failure);
        Console.Error.WriteLine($"Configuration error: {key} - {message}. {fix}");
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
app.UseCorrelationId();
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
        var snapshot = new Dictionary<string, object?>();
        string Mask(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= 2 ? new string('*', value.Length) : new string('*', value.Length - 2) + value[^2..];
        }
        foreach (var kvp in configuration.AsEnumerable())
        {
            if (string.IsNullOrEmpty(kvp.Value)) continue;
            var provider = root.Providers.First(p => p.TryGet(kvp.Key, out _));
            var shouldMask = kvp.Key.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
                             kvp.Key.Contains("Secret", StringComparison.OrdinalIgnoreCase) ||
                             kvp.Key.Equals("ConnectionStrings:Default", StringComparison.OrdinalIgnoreCase);
            var value = shouldMask ? Mask(kvp.Value) : kvp.Value;
            snapshot[kvp.Key] = new { value, source = provider.ToString() };
        }
        return Results.Json(snapshot);
    });

    app.MapGet("/_diag/config/validate", (IServiceProvider sp) =>
    {
        var issues = new List<object>();
        void Capture(Action action)
        {
            try { action(); }
            catch (OptionsValidationException ex)
            {
                foreach (var f in ex.Failures)
                {
                    var parsed = ValidationError.Parse(f);
                    issues.Add(new { Key = parsed.Key, Message = parsed.Message, Fix = parsed.Fix });
                }
            }
        }
        Capture(() => _ = sp.GetRequiredService<IOptions<AstraIdOptions>>().Value);
        Capture(() => _ = sp.GetRequiredService<IOptions<AuthOptions>>().Value);
        Capture(() => _ = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value);
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
