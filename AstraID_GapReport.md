# AstraID Gap Report

Gaps preventing AstraID from being a production‑grade IdP. Priority:
- **P0** – blocker
- **P1** – strongly recommended
- **P2** – roadmap

| Gap | Priority | Evidence | Fix | Acceptance | Risk |
|---|---|---|---|---|---|
| Token validation mode not configured (neither local server nor introspection) | P0 | Validation builder lacks `UseLocalServer()`/`UseIntrospection()`【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L112-L116】 | Add conditional `opt.UseLocalServer()` or `opt.UseIntrospection()` based on `Auth:ValidationMode` | Access token issued by server is accepted; introspection works for remote | Clients could accept unvalidated tokens |
| ASP.NET Identity registration missing in startup | P0 | Program doesn't call `AddIdentityCore` while controllers depend on `UserManager`【F:src/AstraID.Api/Program.cs†L68-L74】 | Register Identity in startup or via `AddAstraIdSecurity` | Admin endpoints resolve `UserManager` and password policy enforced | Authentication APIs fail at runtime |
| Data Protection keys not persisted or configured | P0 | No `AddDataProtection` usage despite entity/table present【chunk:9a466e†L1-L2】 | `services.AddDataProtection().PersistKeysToDbContext<AstraIdDbContext>()` | Keys stored in DB, survive restarts | Token invalidation, inability to decrypt cookies |
| No password/lockout policy enforcement | P1 | Identity options only configured in unused extension【F:src/AstraID.Infrastructure/Extensions/ServiceCollectionExtensions.cs†L20-L27】 | Configure `IdentityOptions.Password`/`Lockout` in startup | Invalid passwords rejected; lockout triggers | Weak credentials, brute-force risk |
| ValidationMode "Introspection" options unused | P1 | `AuthOptions` exposes mode & client credentials but OpenIddictConfig ignores them【F:src/AstraID.Api/Options/AuthOptions.cs†L8-L34】【F:src/AstraID.Api/OpenIddict/OpenIddictConfig.cs†L112-L116】 | When ValidationMode=Introspection, configure `JwtBearer` to use introspection endpoint | Tokens validated remotely | Misconfigured clients, silent failures |
| No correlation ID propagation or logging | P1 | No middleware `UseCorrelationId` or log enrichment【chunk:5a3a94†L1-L9】【F:src/AstraID.Api/Program.cs†L146-L147】 | Add correlation ID middleware and Serilog enrichment | Logs contain `CorrelationId` | Hard to trace requests |
| OpenTelemetry tracing stubbed with no exporter | P1 | Tracing pipeline empty and not wired【F:src/AstraID.Infrastructure/Extensions/ServiceCollectionExtensions.cs†L91-L93】 | Configure OTLP/console exporters and instrument HTTP | Traces visible in collector | Limited observability |
| Security headers static (no CSP nonce, no host restrictions) | P2 | Middleware sets static CSP string【F:src/AstraID.Api/Extensions/SecurityHeadersMiddleware.cs†L22-L33】 | Use `AddCsp` with dynamic nonce and stricter policies | CSP reported in response with nonce | XSS mitigation incomplete |
| No MFA/2FA or external federation | P2 | Two‑factor policies exist but no API/UI | Implement TOTP/WebAuthn flows | 2FA challenge required on login | Account takeover risk |
| Client registration lacks secret hashing & rotation history enforcement | P2 | ClientsController stores secrets directly【F:src/AstraID.Api/Controllers/Admin/ClientsController.cs†L64-L66】 | Hash secrets, enforce rotation policy, track history | Client secret stored hashed with rotation limits | Secret leakage risk |
