# Remediation Plan

## 1. Configure OpenIddict Validation
**Root cause:** Validation is registered with `UseAspNetCore()` only; tokens are not validated against local server or introspection.
**Change snippet:**
```csharp
.AddValidation(opt =>
{
    if (config.GetValue<string>("Auth:ValidationMode") == "Introspection")
    {
        opt.SetIssuer(new Uri(config["AstraId:Issuer"]!));
        opt.UseIntrospection()
           .SetClientId(config["Auth:Introspection:ClientId"]!)
           .SetClientSecret(config["Auth:Introspection:ClientSecret"]!);
    }
    else
    {
        opt.UseLocalServer();
    }
    opt.UseAspNetCore();
});
```
**Acceptance test:** Request a protected API with an invalid token; expect `401` from validation.

## 2. Restrict Development Certificates
**Root cause:** `AddDevelopmentSigningCertificate()`/`AddDevelopmentEncryptionCertificate()` are always used when no certificates configured.
**Change snippet:**
```csharp
var useDevCerts = config.GetValue<bool>("Auth:Certificates:UseDevelopmentCertificates");
if (signingCerts.Any())
    foreach (var cert in signingCerts) opt.AddSigningCertificate(cert);
else if (useDevCerts && env.IsDevelopment())
    opt.AddDevelopmentSigningCertificate();
```
(similar for encryption)
**Acceptance test:** In Production without certificates, startup fails with options validation error.

## 3. Honor Validated CORS/RateLimit Options
**Root cause:** Middleware reads `ASTRAID_ALLOWED_CORS` and `ASTRAID_RATE_LIMIT_*` env vars directly, bypassing validated options.
**Change snippet:**
```csharp
services.AddCors(o =>
    o.AddPolicy("cors", p => p.WithOrigins(options.AllowedCors)
                               .AllowAnyHeader().AllowAnyMethod()));

services.AddRateLimiter(opt =>
{
    opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
        RateLimitPartition.GetTokenBucketLimiter("global", _ =>
            new TokenBucketRateLimiterOptions
            {
                TokenLimit = options.RateLimit.Burst,
                TokensPerPeriod = options.RateLimit.Rps,
                ReplenishmentPeriod = TimeSpan.FromSeconds(1)
            }));
});
```
**Acceptance test:** Setting `AstraId:AllowedCors` in appsettings updates CORS policy after restart.

## 4. Tighten Rate Limiting for OpenID Endpoints
**Root cause:** Only a global rate limiter exists; `/connect/*` endpoints are not specifically throttled.
**Change snippet:**
```csharp
services.AddRateLimiter(options =>
{
    options.AddPolicy("oidc", ctx =>
        RateLimitPartition.GetFixedWindowLimiter(ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));
});
...
app.MapGroup("/connect").RequireRateLimiting("oidc");
```
**Acceptance test:** Rapid requests to `/connect/token` beyond the limit return `429` while other endpoints use global limits.

## 5. Propagate Correlation ID to Logs
**Root cause:** `UseSerilogRequestLogging` is enabled but no correlation-id enricher is configured.
**Change snippet:**
```csharp
builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId());
app.UseCorrelationId();
```
**Acceptance test:** Each log entry contains `CorrelationId`; sending `X-Correlation-ID` header is reflected in logs.

---

Return to [README](README.md)
