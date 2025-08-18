# AstraID Server – Gap Analysis

| Area | Score (0-5) | Observations | Gaps / Actions |
|---|---|---|---|
|Domain|4|Rich aggregates, value objects, policies, domain events|Domain model references EF Core packages directly; consider splitting persistence concerns|
|Application|3|MediatR CQRS with pipeline behaviors|Many commands/queries placeholders; validation coverage incomplete|
|Persistence|4|DbContext with schema, configurations, outbox pattern|No migration scripts committed; design-time factory uses env vars only|
|Infrastructure|3|Repositories, outbox publisher, OpenIddict bridge|Outbox publisher lacks idempotency tracking beyond processed timestamp|
|API|3|Minimal endpoints for users/clients, ProblemDetails|Missing OAuth endpoints like UserInfo, logout; limited management APIs|
|Security|3|HTTPS, JWT auth, CORS allowlist, rate limiting【F:src/AstraID.Api/Extensions/ServiceCollectionExtensions.Security.cs†L1-L54】|PII scrubbing and secret management not fully covered; no Data Protection key persistence implementation|
|Observability|2|Serilog request logging; health checks|OpenTelemetry tracing stubbed (no exporters)【F:src/AstraID.Infrastructure/Extensions/ServiceCollectionExtensions.cs†L88-L93】|
|Ops|2|Env-var driven config; optional migrate/seed【F:src/AstraID.Infrastructure/Startup/WebAppDatabaseExtensions.cs†L22-L54】|No CI/CD pipeline or Dockerfile review; missing runbooks|

## Prioritized Recommendations
- **P0** – Implement missing OAuth endpoints (UserInfo, logout), expand client/user management APIs, and ensure confidential client secrets are hashed before persistence.
- **P1** – Add comprehensive unit/integration tests; include OpenIddict E2E tests for auth flows; configure OpenTelemetry exporters and log filtering to avoid PII.
- **P2** – Separate domain from EF Core dependencies; finalize deployment artifacts (Dockerfile, CI/CD), and document backup/restore and rotation policies.
