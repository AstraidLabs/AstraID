# AstraID Readiness Report

Overall score: **2.5 / 5** – core OAuth flows are present but token validation, identity, and observability gaps remain.

| Item | Status | Evidence |
|---|---|---|
| OpenID Connect endpoints configured | ✅ | Endpoints defined in OpenIddictConfig【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L33-L40】 |
| PKCE (S256) enforced | ✅ | Code challenge methods restricted to SHA256【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L46-L51】 |
| Standard scopes + custom from config | ✅ | RegisterScopes call【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L53-L59】 |
| Token lifetimes, access token format from config | ✅ | Lifetimes and format read from configuration【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L61-L99】 |
| ASP.NET Identity configured with confirmed email | ❌ | AddIdentityCore present but not invoked in Program【F:src/AstraID.Infrastructure/Extensions/ServiceCollectionExtensions.cs†L20-L27】【F:src/AstraID.Api/Program.cs†L68-L74】 |
| Password & lockout policy | ❌ | No IdentityOptions configuration found | |
| Data Protection keys persisted | ❌ | No AddDataProtection usage【chunk:9a466e†L1-L2】 |
| Token validation (local/introspection) | ❌ | AddValidation lacks `UseLocalServer`/`UseIntrospection`【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L112-L116】 |
| CORS allow‑list from config | ✅ | AddAstraIdSecurity reads `ASTRAID_ALLOWED_CORS`【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L20-L26】 |
| Rate limiting enabled | ✅ | Token bucket limiter configured【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L53-L70】 |
| Security headers & HTTPS | ✅ | `UseHttpsRedirection` and HSTS/CSP headers【F:src/AstraID.Api/Program.cs†L145-L152】【F:src/AstraID.Api/Extensions/SecurityHeadersMiddleware.cs†L22-L33】 |
| Secrets masked in logs/diagnostics | ✅ | `_diag/config` masking logic【F:src/AstraID.Api/Program.cs†L159-L177】 |
| Health checks `/health/live` & `/health/ready` | ✅ | HealthChecksConfig maps endpoints【F:src/AstraID.Api/Health/HealthChecksConfig.cs†L14-L27】 |
| Auto-migrate/seed controlled by env flags | ✅ | `ASTRAID_AUTO_MIGRATE` & `ASTRAID_RUN_SEED` gating【F:src/AstraID.Infrastructure/Startup/WebAppDatabaseExtensions.cs†L30-L72】 |
| Serilog request logging | ✅ | `UseSerilogRequestLogging` in Program【F:src/AstraID.Api/Program.cs†L146-L147】 |
| Correlation ID logging | ⚠️ | Not implemented; recommended in FixPlan |
| OpenTelemetry tracing | ⚠️ | Stub only, no exporter configured【F:src/AstraID.Infrastructure/Extensions/ServiceCollectionExtensions.cs†L91-L93】 |
| Comprehensive tests executed | ⚠️ | `dotnet` SDK missing; tests not run（see terminal）【74ab83†L1-L3】 |

**Gaps:** token validation mode, Identity setup, Data Protection key persistence, correlation IDs, OpenTelemetry exporter, and test coverage.

---

See also: [Fix Plan](AstraID_FixPlan.md) · [Test Plan](AstraID_TestPlan.md) · [Security Notes](docs/SECURITY.md)
