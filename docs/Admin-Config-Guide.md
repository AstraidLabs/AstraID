# AstraID Admin Configuration Guide

This guide walks administrators through configuring AstraID using JSON files or environment variables.

## Quick Start
1. Copy the local template:
   - **Linux/macOS:** `cp src/AstraID.Api/appsettings.Local.json.example src/AstraID.Api/appsettings.Local.json`
   - **Windows:** `copy src\AstraID.Api\appsettings.Local.json.example src\AstraID.Api\appsettings.Local.json`
2. Edit `appsettings.Local.json` with your connection string and OAuth introspection credentials.
3. Run the API and verify configuration with `curl http://localhost:5000/_diag/config/validate` (development only).

## Configuration Files
- `appsettings.json` – default values.
- `appsettings.Production.json` – overrides for production deployments.
- `appsettings.Local.json` – local secrets and overrides. This file is **gitignored**.

## Key Settings
| Key | Description | Example |
|-----|-------------|---------|
| `ConnectionStrings:Default` | Database connection string | `Server=localhost;Database=AstraId;User Id=sa;Password=StrongPass;TrustServerCertificate=True` |
| `AstraId:Issuer` | Public base URL of the identity server | `https://id.example.com` |
| `AstraId:AllowedCors` | Array of allowed HTTPS origins for CORS | `["https://app.example.com"]` |
| `Auth:Introspection:ClientId` | Client id used for token introspection | `astra-admin` |
| `Auth:Introspection:ClientSecret` | Secret for introspection client | `p@ssw0rd` |

## Environment Variable Overrides
Any setting can be overridden with environment variables using `__` as the separator.

### Linux/macOS
```bash
export ConnectionStrings__Default="Server=..."
export AstraId__Issuer="https://id.example.com"
```

### Windows PowerShell
```powershell
$env:ConnectionStrings__Default = "Server=..."
$env:AstraId__Issuer = "https://id.example.com"
```

## Troubleshooting
| Symptom | Cause | Fix |
|---------|-------|-----|
| `Configuration error in 'AstraIdOptions': The Issuer field is required.` | Missing `AstraId:Issuer` | Set the value in `appsettings.Local.json` or environment variable `AstraId__Issuer` |
| `/ _diag/config` shows `***` for a value | Secret value is masked | Check your local file or environment variable |
| Validation endpoint returns `ERROR` | One or more settings failed validation | Review the `Message` and update the relevant key |

## More Information
See the Swagger UI "Admin Setup" section when running in development for required keys and links to this guide.
