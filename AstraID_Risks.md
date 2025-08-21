# AstraID Risks

| Risk | Impact | Mitigation |
|---|---|---|
| Tokens may be accepted without proper validation | Unauthorized access | Configure validation mode (local/introspection) and add automated tests |
| Missing DataProtection keys persistence leads to token/cookie invalidation after restart | Users forced to reauthenticate, potential data loss | Persist keys to DB and backup regularly |
| Weak password/lockout policy | Account compromise via brute force | Enforce strong `IdentityOptions` and monitor login attempts |
| Lack of correlation IDs hampers incident investigation | Slower root‑cause analysis | Introduce correlation ID middleware and log enrichment |
| Absent MFA/federation | Higher risk of account takeover | Plan roadmap for TOTP/WebAuthn and external IdP federation |
| Incomplete security headers & CSP | Exposure to XSS/Clickjacking | Harden headers, use nonces, review allowed sources |
| Limited observability (no OpenTelemetry exporter) | Hard to detect performance issues | Configure tracing/metrics exporters and dashboards |
