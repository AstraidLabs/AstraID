# Security Notes

## Secret Management
- **Never commit secrets.** Store secrets in `appsettings.Local.json` (ignored) or environment variables/secret managers.
- Rotate client secrets and database passwords regularly.

## Certificates
- Signing and encryption certificates should be stored as PFX files with strong passwords.
- Rotate certificates before expiration and update `Auth:Certificates` accordingly.
- Each certificate exposes a `kid` in the JWKS endpoint for rotation scenarios.

## Logging & PII
- Serilog writes structured logs; avoid logging personally identifiable information (PII).
- Mask secrets and passwords in logs.

## Rate Limiting
- `AstraId:RateLimit` protects `/connect/*` endpoints. Tune `Rps` and `Burst` for your environment.

## Admin Endpoints
- Administrative APIs require both `astra.admin` scope and `admin` role.
- Review access regularly and audit changes using the built-in audit logging.

## HTTPS & Certificates
- All production deployments must use HTTPS. `AstraId:Issuer` must be an HTTPS URL.

## Additional Guidance
- Use a dedicated account for database access with the least privileges required.
- Enable monitoring and alerts on authentication anomalies.

Refer to [CONFIGURATION.md](CONFIGURATION.md) for details on configuring these features.
