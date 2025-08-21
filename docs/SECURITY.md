# Security Notes

## Secret Management
- **Never commit secrets.** Store secrets in `appsettings.Local.json` (ignored) or environment variables/secret managers.
- Rotate client secrets and database passwords regularly.
- Prefer external vaults for storing secrets and certificate passwords.

## Certificates
- **TLS**: Kestrel loads TLS certificates (PEM/PFX) for HTTPS. Rotate via your platform and ensure private key access is restricted.
- **Token signing/encryption**: Provide PFX files via `Auth:Certificates` for OpenIddict. Rotate certificates before expiration; JWKS exposes their `kid` values.
- Development certificates are only for non‑production use.

## Logging & PII
- Serilog writes structured logs; avoid logging personally identifiable information (PII).
- Mask secrets and passwords in logs.

## Rate Limiting
- `AstraId:RateLimit` protects `/connect/*` endpoints. Tune `Rps` and `Burst` for your environment.
- Use additional reverse‑proxy throttling for network‑level protection.

## Admin Endpoints
- Administrative APIs require both `astra.admin` scope and `admin` role.
- Review access regularly and audit changes using the built-in audit logging.

## HTTPS & Certificates
- All production deployments must use HTTPS. `AstraId:Issuer` must be an HTTPS URL.

## Data Protection
- Data Protection keys are stored in the database. Back up the `auth.DataProtectionKeys` table regularly to avoid token/cookie invalidation.

## Additional Guidance
- Use a dedicated account for database access with the least privileges required.
- Enable monitoring and alerts on authentication anomalies.

---

Refer to [CONFIGURATION.md](CONFIGURATION.md) for details on configuring these features. Return to [Install](INSTALL.md) · [Troubleshooting](TROUBLESHOOTING.md).
