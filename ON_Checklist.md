# ON Checklist

## A. OpenIddict Server
- ❌ Validation pipeline configured (local or introspection)
- ✅ Authorization Code, Refresh Token, Client Credentials grants
- ✅ PKCE with S256 only
- ⚠ Certificates fall back to development keys in all environments
- ✅ Access token format configurable (Jwt/Reference)
- ✅ PAR endpoint registered; enforced via `AstraId:RequireParForPublicClients`

## B. OpenIddict Validation
- ❌ Missing `UseLocalServer()` or `UseIntrospection()` configuration

## C. ASP.NET Core Pipeline
- ✅ HTTPS redirection & HSTS via security headers
- ⚠ Serilog request logging lacks correlation id enrichment
- ⚠ CORS origins and rate limits sourced from env vars, not validated options
- ⚠ Rate limiting not tightened for `/connect/*`
- ✅ ProblemDetails middleware registered after auth

## D. Discovery & Metadata
- ⚠ Discovery document not captured (environment lacked .NET SDK)

## E. Error Handling
- ✅ ProblemDetails for non-OAuth APIs
- ⚠ No evidence of RFC-compliant error handling for introspection/revocation endpoints

## F. Configuration
- ✅ Options bound with `ValidateOnStart`
- ⚠ `AllowedCors` and `RateLimit` options validated but unused in services
- ✅ Connection string, issuer, token lifetimes validated

## G. Security Extras
- ✅ Auto-migrate and seed guarded by env vars
- ✅ Admin endpoints require roles/scopes
- ⚠ Correlation ID not propagated to logs

## H. Tests
- ⚠ Only basic discovery/JWKS/userinfo tests; missing auth code, refresh, client credentials, introspection, revocation and negative cases

---

Return to [README](README.md)
