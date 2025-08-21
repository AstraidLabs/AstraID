# AstraID "What must be ON?" Audit

## Summary Matrix
| Area | Item | Status |
|------|------|--------|
| A. OpenIddict Server | Authorization Code, Refresh, Client Credentials grants | ✅ |
|                      | PKCE S256 only | ✅ |
|                      | Standard + config scopes | ✅ |
|                      | Token lifetimes from config | ✅ |
|                      | Certs fallback only in Development | ⚠ |
|                      | Access token format configurable | ✅ |
|                      | PAR registered/enforced | ✅ |
|                      | UserInfo endpoint passthrough | ✅ |
| B. Validation | Local server or introspection configured | ❌ |
| C. Pipeline | Required middleware order | ⚠ |
| D. Discovery | Discovery document captured | ⚠ |
| E. Error Handling | RFC errors for OAuth endpoints | ⚠ |
| F. Configuration | Options bound & validated | ✅ |
| G. Security Extras | Migration/seeding guarded | ✅ |
| H. Tests | Coverage for auth flows and negatives | ⚠ |

---
## A. OpenIddict Server
- **Status:** Mostly ON
- **Why it matters:** OpenIddict must expose required endpoints and enforce secure flows.
- **Evidence:**
  - Endpoints & grants configured with PKCE and S256 only【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L33-L51】
  - Scopes and token lifetimes bound from configuration【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L53-L67】
  - Development certs added unconditionally when none provided【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L69-L89】
  - Access token format and PAR support from config【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L91-L104】
  - UserInfo passthrough enabled【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L106-L110】
- **Fix:** Guard development certificates so they are only used when `UseDevelopmentCertificates=true` *and* environment is Development (see ON_Fixes.md §2).
- **Acceptance test:** In Production without certs, startup fails; with certs provided, tokens are signed with configured certificate.

## B. OpenIddict Validation
- **Status:** OFF
- **Why it matters:** Without validation configuration, issued tokens are not checked, allowing unauthorized access.
- **Evidence:** Validation builder only calls `.UseAspNetCore()` without `UseLocalServer` or `UseIntrospection`【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L112-L116】
- **Fix:** Configure validation based on `Auth:ValidationMode` (see ON_Fixes.md §1).
- **Acceptance test:** Invalid token request returns `401`.

## C. ASP.NET Core Pipeline
- **Status:** Partially ON
- **Why it matters:** Middleware order influences security guarantees and CORS/rate-limit enforcement.
- **Evidence:** Actual pipeline order【F:src/AstraID.Api/Program.cs†L141-L152】; CORS and rate limiter options sourced from environment variables【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L20-L69】
- **Fix:** Pull CORS and rate-limit settings from validated options, add correlation-id enrichment, and add `/connect/*` rate-limit policy (see ON_Fixes.md §§3-5).
- **Acceptance test:** Changing `AstraId:AllowedCors` updates policy; requests to `/connect/token` beyond limit return `429`; logs show correlation IDs.

## D. Discovery & Metadata
- **Status:** ⚠
- **Why it matters:** Clients rely on discovery for endpoint URIs and supported features.
- **Evidence:** Discovery document could not be captured because .NET 9 SDK was missing (see ON_Discovery_Snapshot.json).
- **Fix:** Install .NET 9 SDK and run the API to verify discovery document contains all required metadata.
- **Acceptance test:** `curl https://server/.well-known/openid-configuration` returns document including `authorization_endpoint`, `token_endpoint`, `jwks_uri`, etc.

## E. Error Handling
- **Status:** Partially ON
- **Why it matters:** RFC-compliant errors enable clients to react correctly; ProblemDetails standardizes API responses.
- **Evidence:** ProblemDetails mappings for API errors【F:src/AstraID.Api/Extensions/ProblemDetailsExtensions.cs†L18-L45】; no passthrough for introspection endpoint in OpenIddict config【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L33-L40】
- **Fix:** Enable passthrough for introspection/revocation if custom handling needed; verify OAuth endpoints return `error`/`error_description`.
- **Acceptance test:** Hitting `/connect/introspect` with invalid token returns RFC error JSON.

## F. Configuration
- **Status:** ON with gaps
- **Why it matters:** Misconfiguration leads to insecure deployments or runtime failures.
- **Evidence:** Options bound with `ValidateOnStart`【F:src/AstraID.Api/Program.cs†L49-L66】; CORS/rate-limit options validated but unused at runtime【F:src/AstraID.Api/Options/AstraIdOptions.cs†L12-L35】【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L20-L69】
- **Fix:** Use `AstraIdOptions` values when configuring CORS and rate limiting (ON_Fixes.md §3).
- **Acceptance test:** misconfigured CORS origin causes startup validation failure instead of runtime surprise.

## G. Security Extras
- **Status:** ON with minor gaps
- **Why it matters:** Protects administrative operations and sensitive data.
- **Evidence:** Auto-migrate/seeding controlled by flags【F:src/AstraID.Infrastructure/Startup/WebAppDatabaseExtensions.cs†L27-L52】; admin endpoints require policies【F:src/AstraID.Api/Extensions/ApplicationEndpoints.Users.cs†L16-L33】【F:src/AstraID.Api/Extensions/ApplicationEndpoints.Clients.cs†L18-L44】; no correlation-id propagation found.
- **Fix:** Add correlation-id middleware and Serilog enricher (ON_Fixes.md §5).
- **Acceptance test:** Logs show `CorrelationId` and admin endpoints return `403` when unauthorized.

## H. Tests
- **Status:** ⚠
- **Why it matters:** Automated tests verify critical auth flows and prevent regressions.
- **Evidence:** Integration tests cover discovery, JWKS, and basic CRUD only【F:tests/AstraID.IntegrationTests/OidcTests.cs†L55-L89】
- **Fix:** Add tests for Authorization Code + PKCE, Refresh Token, Client Credentials, UserInfo, Introspection, Revocation, and negative scenarios.
- **Acceptance test:** `dotnet test` executes new suites covering positive and negative OAuth flows.
