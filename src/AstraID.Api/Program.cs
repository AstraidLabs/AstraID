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

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
builder.Configuration.AddUserSecrets<Program>(optional: true);
#endif

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

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
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "AstraID API", Version = "v1" });
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

var app = builder.Build();

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
}

app.MapControllers();
app.MapUserEndpoints();
app.MapClientEndpoints();
app.MapAstraIdHealthChecks();

await app.UseAstraIdDatabaseAsync();

app.Run();
