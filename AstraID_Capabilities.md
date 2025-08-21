# AstraID Capabilities

Inventory of implemented features with evidence links.

## OpenID Connect & OAuth2

| Feature | Evidence |
|---|---|
| Authorization, Token, Introspection, UserInfo, Revocation, Logout, JWKS, PAR endpoints wired | `OpenIddictConfig` defines all endpoint URIs and enables flows【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L33-L40】 |
| Authorization Code + PKCE (S256), Refresh Token, Client Credentials grants | Flows and PKCE enforcement configured【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L42-L51】 |
| Standard & custom scopes | Standard set plus config-driven custom scopes registered【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L53-L59】 |
| Token lifetimes from configuration | Access/identity/refresh lifetimes set from `Auth:TokenLifetimes`【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L61-L67】 |
| Signing/encryption certificates with dev fallback | Production certificates loaded or dev certs used【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L69-L89】 |
| Access token format selectable (JWT or reference) | Controlled by `Auth:AccessTokenFormat`【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L91-L99】 |
| Optional PAR enforcement | Config flag `AstraId:RequireParForPublicClients`【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L101-L104】 |

## Authentication & Identity

| Feature | Evidence |
|---|---|
| Admin APIs for users, roles, clients | REST controllers available under `/api/admin`【F:src/AstraID.Api/Controllers/Admin/UsersController.cs†L16-L76】【F:src/AstraID.Api/Controllers/Admin/RolesController.cs†L13-L56】【F:src/AstraID.Api/Controllers/Admin/ClientsController.cs†L15-L115】 |
| Email confirmation required | Identity options set via `AddIdentityCore` (not wired in Program) to require confirmed email【F:src/AstraID.Infrastructure/Extensions/ServiceCollectionExtensions.cs†L20-L27】 |

## Security Hardening

| Feature | Evidence |
|---|---|
| HTTPS redirection, HSTS & security headers | Middleware sets HSTS/headers and Program enforces HTTPS【F:src/AstraID.Api/Program.cs†L145-L152】【F:src/AstraID.Api/Extensions/SecurityHeadersMiddleware.cs†L22-L33】 |
| CORS allow‑list from config | Origins read from `ASTRAID_ALLOWED_CORS`【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L20-L26】 |
| Rate limiting | Token‑bucket limiter configured from config【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L53-L70】 |
| Secrets masked in diagnostics | `_diag/config` masks passwords and secrets【F:src/AstraID.Api/Program.cs†L159-L177】 |
| Auto‑migration/seed gated by env flags | `ASTRAID_AUTO_MIGRATE` & `ASTRAID_RUN_SEED` checked at startup【F:src/AstraID.Infrastructure/Startup/WebAppDatabaseExtensions.cs†L30-L72】 |

## Configuration & Options

| Feature | Evidence |
|---|---|
| Config‑first (`appsettings` + env overrides) | Program loads JSON + env + optional JSONC【F:src/AstraID.Api/Program.cs†L19-L39】 |
| Strong options validation | Options bound and validated on start with custom validators【F:src/AstraID.Api/Program.cs†L49-L66】 |
| Developer diagnostics | `_diag/config` and `_diag/config/validate` endpoints in Development【F:src/AstraID.Api/Program.cs†L154-L201】 |

## Persistence & Migrations

| Feature | Evidence |
|---|---|
| EF Core 9 DbContext with OpenIddict integration | `AstraIdDbContext` uses Identity & OpenIddict and default schema `auth`【F:src/AstraID.Persistence/AstraIdDbContext.cs†L16-L34】 |
| Pluggable SQL Server/Postgres provider | Connection provider selected via `ASTRAID_DB_PROVIDER`【F:src/AstraID.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs†L25-L34】 |

## Observability & Ops

| Feature | Evidence |
|---|---|
| Serilog with request logging | Serilog configured from config and middleware enabled【F:src/AstraID.Api/Program.cs†L47】【F:src/AstraID.Api/Program.cs†L146-L147】 |
| Health endpoints | `/health/live` and `/health/ready` mapped【F:src/AstraID.Api/Health/HealthChecksConfig.cs†L14-L27】 |

---

See also: [Feature Inventory](AstraID_FeatureInventory.md) · [Traceability](AstraID_Traceability.md) · [Readiness Report](AstraID_ReadinessChecklist.md)
