# Key Rotation and TLS Certificate Runbook

## Timeline
- **Generation**: `SigningKeyRotationService` checks certificate folders hourly.
- **Rotation**: When the active key's `rotateAfter` has passed, a new ECDSA P-256 certificate is created and stored as PFX.
- **Promotion**: The new key becomes **Active** and previous active keys are marked **Grace** for `GraceDays`.
- **Recycle**: The service signals `StopApplication()` so OpenIddict reloads the key set on restart.
- **Pruning**: Keys in **Grace** are removed from disk and manifest once `graceUntil` expires.

## Operational Runbook
1. Configure paths in `AstraId:Certificates` and `AstraId:LetsEncrypt` in `appsettings`.
2. Ensure Let's Encrypt PEM files (`fullchain.pem`, `privkey.pem`) exist in the configured `CertPath`.
3. Kestrel reloads TLS certificates automatically on PEM change; no restart required.
4. Signing certificates rotate automatically; monitor logs for `Generating new certificate` events.
5. During grace period JWKS exposes both old and new keys. Clients must refresh JWKS regularly.
6. If rotation fails, inspect manifest and certificate folders, then manually trigger recycle after fixing.
