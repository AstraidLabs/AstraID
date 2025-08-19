# Configuration Reference

AstraID uses a **config-first** model. All settings are defined in configuration files or environment variables.

## Load Order

Settings are loaded in the following order (later entries override earlier ones):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. `appsettings.Local.json` (not committed)
4. Environment variables

## ConnectionStrings

| Key | Description | Example |
|-----|-------------|---------|
| `ConnectionStrings:Default` | SQL Server connection string. Must include either `Integrated Security=True` or `User Id` and `Password`. | `Server=localhost\\SQLEXPRESS;Database=AstraID;Integrated Security=True;Encrypt=True;TrustServerCertificate=True` |

## AstraId

| Key | Description | Example |
|-----|-------------|---------|
| `AstraId:Issuer` | Base HTTPS URL of the server. | `https://id.example.com` |
| `AstraId:AllowedCors[]` | Array of allowed origins. Use absolute URLs. | `["https://app.example.com"]` |
| `AstraId:RateLimit:Rps` | Requests per second limit (1–1000). | `10` |
| `AstraId:RateLimit:Burst` | Burst limit (1–5000, >= Rps). | `20` |
| `AstraId:AutoMigrate` | Apply EF migrations at startup. | `false` |
| `AstraId:RunSeed` | Seed test data at startup. | `false` |
| `AstraId:RequireParForPublicClients` | Require Pushed Authorization Requests for public clients. | `false` |

## Auth

| Key | Description | Example |
|-----|-------------|---------|
| `Auth:ValidationMode` | `Jwt` (default) or `Introspection`. | `Jwt` |
| `Auth:AccessTokenFormat` | `Jwt` (default) or `Reference`. | `Jwt` |
| `Auth:TokenLifetimes:AccessMinutes` | Access token lifetime in minutes (1–240). | `60` |
| `Auth:TokenLifetimes:IdentityMinutes` | ID token lifetime in minutes (1–240). | `15` |
| `Auth:TokenLifetimes:RefreshDays` | Refresh token lifetime in days (1–90). | `14` |
| `Auth:Certificates:UseDevelopmentCertificates` | Use self-signed development certs. | `true` |
| `Auth:Certificates:Signing[]` | Array of signing certs (Path, Password). | `{ "Path": "C:\\certs\\signing.pfx", "Password": "CHANGE_ME" }` |
| `Auth:Certificates:Encryption[]` | Array of encryption certs. | `{ "Path": "C:\\certs\\enc.pfx", "Password": "CHANGE_ME" }` |
| `Auth:Introspection:ClientId` | Required when `ValidationMode=Introspection`. | `admin-cli` |
| `Auth:Introspection:ClientSecret` | Secret for introspection client. | `CHANGE_ME` |

## Environment Variable Overrides

Use double underscores (`__`) instead of colons (`:`) and indexes for arrays.

| Setting | Windows PowerShell | Linux/macOS |
|---------|-------------------|-------------|
| `ConnectionStrings:Default` | `$env:ConnectionStrings__Default="Server=.\\SQLEXPRESS;Database=AstraID;Integrated Security=True"` | `export ConnectionStrings__Default="Server=.\\SQLEXPRESS;Database=AstraID;Integrated Security=True"` |
| `AstraId:Issuer` | `$env:AstraId__Issuer="https://id.example.com"` | `export AstraId__Issuer="https://id.example.com"` |
| `AstraId:AllowedCors[0]` | `$env:AstraId__AllowedCors__0="https://app.example.com"` | `export AstraId__AllowedCors__0="https://app.example.com"` |
| `Auth:Introspection:ClientSecret` | `$env:Auth__Introspection__ClientSecret="CHANGE_ME"` | `export Auth__Introspection__ClientSecret="CHANGE_ME"` |

## Validation Rules

AstraID validates configuration on startup and fails fast if rules are violated:

- `AstraId:Issuer` must be an absolute **HTTPS** URL.
- `AstraId:AllowedCors` entries must be absolute URLs using HTTPS (HTTP allowed only in Development).
- `AstraId:RateLimit:Rps` must be between 1 and 1000; `Burst` between 1 and 5000 and >= `Rps`.
- `Auth:Certificates` paths must exist unless `UseDevelopmentCertificates=true`.
- `ConnectionStrings:Default` must include credentials or `Integrated Security=True`.

Edit [`docs/SAMPLES/appsettings.Local.json.example`](SAMPLES/appsettings.Local.json.example) for a full template.
