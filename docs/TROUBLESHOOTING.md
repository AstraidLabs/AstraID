# Troubleshooting

Common issues and their fixes when running AstraID.

## Discovery Endpoint 404 or Empty
- Ensure `AstraId:Issuer` matches the external URL.
- Verify the application is running and listening on the configured port.

## PKCE Errors
- AstraID only accepts the `S256` code challenge method.
- Clients must send `code_challenge_method=S256` and a matching `code_verifier`.

## Login or Token Failures
- Confirm the client is registered with the correct redirect URIs and scopes.
- For client credentials, verify the client secret and `scope` value.

## Database Connection Issues
- Check the connection string syntax. Escape backslashes in JSON: `Server=HOST\\SQLEXPRESS`.
- For self-signed certificates, add `TrustServerCertificate=True`.

## Certificate Load Failures
- Paths in `Auth:Certificates` must exist and be accessible by the process.
- Ensure the password is correct. In Development you can set `UseDevelopmentCertificates=true`.

## CORS Blocked
- Add origins to `AstraId:AllowedCors` using absolute URLs.
- Remember that HTTP origins are only allowed in Development.

## Health Endpoint Shows "unready"
- `/health/ready` aggregates checks for the database, data protection, and certificates.
- Check application logs to determine which probe failed.

For additional help, open an issue with detailed logs and configuration snippets.
