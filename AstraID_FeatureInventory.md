# AstraID Server – Feature Inventory

| Area | Feature | Details | Source |
|---|---|---|---|
|Protocols|OAuth2/OpenID Connect|Authorization Code (PKCE), Client Credentials, Refresh Token|OpenIddict configuration【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L22-L37】|
|Tokens|ID, Access, Refresh|Issued via OpenIddict|README【F:README.md†L23-L27】|
|User Management|Register user, get user by ID|Minimal API endpoints calling MediatR handlers【F:src/AstraID.Api/Extensions/ApplicationEndpoints.Users.cs†L13-L34】|
|Client Management|Register client, rotate secret, get client|API endpoints with authorization policies【F:src/AstraID.Api/Extensions/ApplicationEndpoints.Clients.cs†L13-L40】|
|Authorization|Scope & role policies, AuthorizationBehavior|Application pipeline enforces scopes/roles【F:src/AstraID.Application/Behaviors/AuthorizationBehavior.cs†L1-L24】|
|Domain Events|Outbox pattern|EF interceptor stores events to outbox; background publisher dispatches【F:src/AstraID.Infrastructure/Persistence/Interceptors/DomainEventsCollectorInterceptor.cs†L1-L44】【F:src/AstraID.Infrastructure/Messaging/Background/OutboxHostedService.cs†L1-L27】|
|Security|HTTPS/HSTS, security headers, rate limiting, JWT auth|Middleware and services added in Program/Extensions【F:src/AstraID.Api/Program.cs†L35-L49】【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L1-L54】|
|Problem Handling|ProblemDetails mappings for validation, unauthorized, server errors|ProblemDetailsExtensions【F:src/AstraID.Api/Extensions/ProblemDetailsExtensions.cs†L17-L44】|
|Health|Liveness and readiness probes|`/health/live` and `/health/ready` endpoints【F:src/AstraID.Api/Health/HealthChecksConfig.cs†L15-L23】|
|Seeding/Migration|Optional DB migrate and seed via env flags|`ASTRAID_AUTO_MIGRATE`, `ASTRAID_RUN_SEED`【F:src/AstraID.Infrastructure/Startup/WebAppDatabaseExtensions.cs†L22-L54】|
|Logging|Serilog request logging|Enabled in Program.cs【F:src/AstraID.Api/Program.cs†L37】|
