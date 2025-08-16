// Entry point configuring the HTTP pipeline and DI for AstraID.
//
// Key responsibilities:
// - Compose the service container with persistence, identity and OpenIddict modules.
// - Establish middleware order to enforce HTTPS, CORS and authentication.
// - Bootstraps Serilog and health checks for observability.
//
// Why this exists: centralizes host configuration for the minimal API application.
// Gotchas: middleware order matters for security; environment flags control seeding and swagger.

using AstraID.Infrastructure.Extensions;
using AstraID.Infrastructure.DependencyInjection;
using AstraID.Infrastructure.Startup;
using AstraID.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddAstraIdPersistence(builder.Configuration)
                .AddIdentityAndAuth(builder.Configuration)
                .AddOpenIddictServer(builder.Configuration)
                .AddTelemetry(builder.Configuration)
                .AddAstraIdOptions(builder.Configuration);

builder.Services.AddCors(options =>
{
    // Rationale: CORS is locked to configured origins to avoid token leakage to arbitrary sites.
    var origins = (builder.Configuration["ASTRAID_ALLOWED_CORS"] ?? string.Empty)
        .Split(';', StringSplitOptions.RemoveEmptyEntries);
    options.AddPolicy("cors", p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning();

#if DEBUG
builder.Configuration.AddUserSecrets<Program>(optional: true); // Avoid storing secrets in source control.
#endif

var app = builder.Build();

// Apply optional auto-migrate & seed based on environment flags
// Rationale: simplifies dev/test setup but should be disabled in production.
await app.UseAstraIdDatabaseAsync();

app.UseSerilogRequestLogging();

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Enforce HTTPS in production to mitigate downgrade attacks.
}
app.UseHttpsRedirection();
app.UseCors("cors");
app.UseAuthentication(); // Must precede authorization.
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();
