# AstraID Fix Plan

Ordered implementation steps to close gaps.

## P0 – Blockers

1. **Enable token validation**
   ```csharp
   // in OpenIddictConfig.AddAstraIdOpenIddict
   .AddValidation(opt =>
   {
       if (configuration.GetValue<string>("Auth:ValidationMode") == "Introspection")
           opt.UseIntrospection().SetClientId(auth.Introspection.ClientId).SetClientSecret(auth.Introspection.ClientSecret);
       else
           opt.UseLocalServer();
       opt.UseAspNetCore();
   });
   ```
2. **Register ASP.NET Identity and DataProtection**
   ```csharp
   // in Program before OpenIddict
   builder.Services.AddDataProtection()
       .PersistKeysToDbContext<AstraIdDbContext>();
   builder.Services.AddIdentityCore<AppUser>(o =>
       {
           o.User.RequireUniqueEmail = true;
           o.SignIn.RequireConfirmedEmail = true;
       })
       .AddRoles<AppRole>()
       .AddEntityFrameworkStores<AstraIdDbContext>()
       .AddDefaultTokenProviders();
   ```

## P1 – Strongly Recommended

1. **Enforce password & lockout policies**
   ```csharp
   builder.Services.Configure<IdentityOptions>(o =>
   {
       o.Password.RequiredLength = 12;
       o.Lockout.MaxFailedAccessAttempts = 5;
   });
   ```
2. **Wire ValidationMode:Introspection**
   ```json
   // appsettings.json
   "Auth": {
     "ValidationMode": "Introspection",
     "Introspection": {"ClientId": "id","ClientSecret": "secret"}
   }
   ```
3. **Correlation IDs & Serilog enrichment**
   ```csharp
   builder.Services.AddRouting();
   builder.Services.AddCorrelationId();
   builder.Host.UseSerilog((ctx, cfg) => cfg
       .ReadFrom.Configuration(ctx.Configuration)
       .Enrich.WithCorrelationId());
   app.UseCorrelationId();
   ```
4. **OpenTelemetry exporters**
   ```csharp
   services.AddOpenTelemetry()
       .WithTracing(b => b
           .AddAspNetCoreInstrumentation()
           .AddSqlClientInstrumentation()
           .AddOtlpExporter());
   ```

## P2 – Roadmap

1. **Harden security headers**
   ```csharp
   context.Response.Headers["Content-Security-Policy"] =
       $"default-src 'self'; script-src 'self' 'nonce-{nonce}'; object-src 'none'";
   ```
2. **Implement MFA/WebAuthn** – expose endpoints to register authenticators and require TOTP/WebAuthn for sensitive operations.
3. **Client secret management** – hash secrets on save and store rotation history.
