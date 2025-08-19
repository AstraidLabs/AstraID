# AstraID Admin Configuration Guide

This guide walks administrators through configuring AstraID using JSON files or environment variables.

## Quick Start
1. Copy the local template:
   - **Linux/macOS:** `./scripts/config-new-local.sh`
   - **Windows:** `powershell .\scripts\config-new-local.ps1`
2. Edit `appsettings.Local.json` and replace all `CHANGE_ME` placeholders.
3. Run the API and verify configuration with `curl http://localhost:5000/_diag/config/validate` (development only).

## Configuration Files
- `appsettings.json` – default values.
- `appsettings.Production.json` – overrides for production deployments.
- `appsettings.Local.json` – local secrets and overrides. This file is **gitignored**.

## Key Settings
| Key | Description | Example |
|-----|-------------|---------|
| `ConnectionStrings:Default` | Database connection string | `Server=YOUR_HOST\SQLEXPRESS;Database=AstraID;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;` |
| `AstraId:Issuer` | Public base URL of the identity server (HTTPS) | `https://id.example.com` |
| `AstraId:AllowedCors` | Array of allowed browser origins | `["https://app.example.com"]` |
| `Auth:Certificates:Signing[n].Path` | Path to a PFX signing cert | `C:\\certs\\astraid-signing.pfx` |
| `Auth:Introspection:ClientId` | (When ValidationMode = Introspection) client id for introspection | `admin-cli` |
| `Auth:Introspection:ClientSecret` | (When ValidationMode = Introspection) client secret | `p@ssw0rd` |

## Environment Variable Overrides
Any setting can be overridden with environment variables using `__` as the separator.

### Linux/macOS
```bash
export ConnectionStrings__Default="Server=..."
export AstraId__Issuer="https://id.example.com"
export AstraId__AllowedCors__0="https://app.example.com"
export Auth__Introspection__ClientId="admin-cli"
export Auth__Introspection__ClientSecret="s3cr3t"
```

### Windows PowerShell
```powershell
$env:ConnectionStrings__Default = "Server=..."
$env:AstraId__Issuer = "https://id.example.com"
$env:AstraId__AllowedCors__0 = "https://app.example.com"
$env:Auth__Introspection__ClientId = "admin-cli"
$env:Auth__Introspection__ClientSecret = "s3cr3t"
```

## Troubleshooting
| Symptom | Cause | Fix |
|---------|-------|-----|
| `Configuration error in 'AstraIdOptions': The Issuer field is required.` | Missing `AstraId:Issuer` | Set the value in `appsettings.Local.json` or environment variable `AstraId__Issuer` |
| `/ _diag/config` shows `***` for a value | Secret value is masked | Check your local file or environment variable |
| Validation endpoint returns `ERROR` | One or more settings failed validation | Review the `Message` and update the relevant key |

## More Information
See the Swagger UI "Admin Setup" section when running in development for required keys and links to this guide.
