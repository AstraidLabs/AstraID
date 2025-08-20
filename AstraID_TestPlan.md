# AstraID Test Plan

## Positive Cases

| Flow | Steps | Expected Result |
|---|---|---|
| Authorization Code + PKCE | 1. Start auth request with S256 `code_challenge`<br>2. Exchange code with matching `code_verifier` | `access_token`, `id_token`, and optional `refresh_token` returned |
| Refresh Token | 1. Obtain refresh token from previous flow<br>2. POST `/connect/token` with `grant_type=refresh_token` | New access token issued; old refresh token invalidated if rotation enabled |
| Client Credentials | POST `/connect/token` with confidential client's id/secret | Access token with configured scopes |
| UserInfo | Call `/connect/userinfo` with bearer access token | Claims match requested scopes |
| Introspection | POST `/connect/introspect` with confidential credentials | `active:true` for valid token, metadata returned |
| Revocation | POST `/connect/revocation` with refresh token | Token cannot be used afterwards |
| Discovery | GET `/.well-known/openid-configuration` | Document contains `issuer`, endpoints, `jwks_uri` |
| Health | GET `/health/ready` when DB reachable and keys present | 200 OK |

## Negative Cases

| Scenario | Steps | Expected Result |
|---|---|---|
| Bad redirect URI | Send `redirect_uri` not registered for client | Authorization endpoint returns `error=invalid_request` |
| Missing/wrong `code_challenge` | Omit or use `plain` challenge | Authorization endpoint returns `error=invalid_request` |
| Unknown scope | Request unconfigured scope | Token endpoint returns `error=invalid_scope` |
| Expired authorization code | Reuse code after first exchange | Token endpoint returns `error=invalid_grant` |
| Revocation idempotency | Revoke same token twice | Second call returns 200 with no side effects |
| Disallowed CORS origin | Browser request from unlisted origin | Preflight blocked with 403 |
| Rate limit exceeded | Exceed configured RPS to `/connect/token` | 429 Too Many Requests |
| Invalid configuration | Start with `AstraId:Issuer` using HTTP or missing DB conn | App startup fails with validation error |

## Test Types
- **Unit** – domain behaviors (e.g., secret rotation, lockout policy).
- **Integration** – API endpoints using in-memory server and EF Core in-memory/SQLite.
- **End-to-End** – full OAuth flows against running server with HTTP client.

## Pass Criteria
- All positive cases return expected HTTP status and payload.
- Negative cases yield RFC-compliant error responses.
- Health checks report `Healthy` when dependencies available.
