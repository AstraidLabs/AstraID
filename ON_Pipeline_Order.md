# Pipeline Order

## Effective Order (top → bottom)
1. `UseForwardedHeaders`
2. `UseHttpsRedirection`
3. `UseSecurityHeaders`
4. `UseSerilogRequestLogging`
5. `UseCors("cors")`
6. `UseRateLimiter`
7. `UseAuthentication`
8. `UseAuthorization`
9. `UseProblemDetails`
10. Endpoint mappings (`MapControllers`, `MapUserEndpoints`, `MapClientEndpoints`)
11. Health checks (`MapAstraIdHealthChecks`)

## Required Order
1. HTTPS redirection & HSTS
2. Security headers
3. Serilog request logging (with correlation id)
4. CORS (allowlist from config)
5. Rate limiting (stricter on `/connect/*`)
6. Authentication
7. Authorization
8. ProblemDetails
9. Map endpoints (OpenIddict + management API)
10. Health checks

## Diff
- ✅ Order of middleware largely matches requirements.
- ⚠ `UseForwardedHeaders` precedes HTTPS redirection; ensure forwarded headers configuration matches proxy setup.
- ⚠ No explicit correlation-id enrichment in Serilog request logging.
- ⚠ CORS and rate limiting draw settings from environment variables instead of validated options.
- ⚠ Rate limiting is global; no special policy for `/connect/*`.
