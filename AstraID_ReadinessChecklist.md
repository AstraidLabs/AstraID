# AstraID Server – Production Readiness Checklist

| Item | Status | Evidence |
|---|---|---|
|.NET 9, nullable enabled|⚠️|Projects target net9.0 but SDK not installed in CI yet【F:src/AstraID.Application/AstraID.Application.csproj†L4-L8】|
|MediatR pipeline (Validation → Authorization → UoW → Logging)|✅|DI registration order【F:src/AstraID.Application/DependencyInjection/ServiceCollectionExtensions.cs†L16-L25】|
|OpenIddict configured with PKCE & client credentials|✅|OpenIddictConfig sets flows and requires PKCE【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L22-L37】|
|Domain events to outbox & publisher|✅|Interceptor and hosted service implemented【F:src/AstraID.Infrastructure/Persistence/Interceptors/DomainEventsCollectorInterceptor.cs†L1-L44】【F:src/AstraID.Infrastructure/Messaging/Background/OutboxHostedService.cs†L1-L27】|
|ProblemDetails mappings for validation & unauthorized|✅|ProblemDetailsExtensions【F:src/AstraID.Api/Extensions/ProblemDetailsExtensions.cs†L17-L44】|
|Health checks (/health/live, /health/ready)|✅|HealthChecksConfig maps endpoints【F:src/AstraID.Api/Health/HealthChecksConfig.cs†L15-L23】|
|Automatic DB migrations disabled by default|✅|WebAppDatabaseExtensions requires env flag `ASTRAID_AUTO_MIGRATE`【F:src/AstraID.Infrastructure/Startup/WebAppDatabaseExtensions.cs†L22-L41】|
|Data protection key persistence|❌|No implementation found (only entity defined)|
|PII-safe logging & secrets handling|⚠️|Serilog configured; no explicit PII scrubbers| 
|Comprehensive tests|❌|Test projects present but tooling unavailable in environment|
