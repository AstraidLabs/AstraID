# Installation Guide

This guide walks administrators through installing **AstraID** on Windows, Linux, or macOS without Docker.

## Prerequisites

| Component | Version | Notes |
|-----------|---------|-------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 9.0 | includes `dotnet` CLI |
| [SQL Server](https://www.microsoft.com/sql-server) | 2019+ | Express/Developer editions work; LocalDB for dev |
| EF Core Tools | 9.0 | `dotnet tool install --global dotnet-ef` |
| PowerShell or Bash | – | used for CLI commands |
| Certificates (optional) | PFX | TLS and token signing/encryption certificates |

## 1. Clone and Restore

```bash
# Windows PowerShell
 git clone https://github.com/AstraID/AstraID.git
 cd AstraID
 dotnet restore
```

```bash
# Linux/macOS
 git clone https://github.com/AstraID/AstraID.git
 cd AstraID
 dotnet restore
```

## 2. Configure

1. Copy the sample configuration.

   ```bash
   cp docs/SAMPLES/appsettings.Local.example.json src/AstraID.Api/appsettings.Local.json
   ```

2. Edit `src/AstraID.Api/appsettings.Local.json` to set the connection string and other options.
   - For Windows authentication:
     ```json
     "ConnectionStrings": {
       "Default": "Server=localhost\\SQLEXPRESS;Database=AstraID;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
     }
     ```
   - For SQL authentication:
     ```json
     "ConnectionStrings": {
       "Default": "Server=tcp:db.example.com,1433;Database=AstraID;User Id=CHANGE_ME;Password=CHANGE_ME;Encrypt=True;TrustServerCertificate=False"
     }
     ```

## 3. Create the Database

AstraID ships with EF Core migrations. Apply them before first run and whenever new migrations are added.

```bash
# Apply migrations
dotnet ef database update -p src/AstraID.Persistence -s src/AstraID.Api
```

To add new migrations later:

```bash
dotnet ef migrations add <Name> -p src/AstraID.Persistence -s src/AstraID.Api
dotnet ef database update -p src/AstraID.Persistence -s src/AstraID.Api
```

## 4. Run

```bash
# Start the API (https://localhost:5001)
dotnet run -p src/AstraID.Api --launch-profile https
```

## 5. Verify

Check the following endpoints:

| Endpoint | Purpose |
|----------|---------|
| `/.well-known/openid-configuration` | OpenID Connect discovery |
| `/connect/token` | Token endpoint (test with client credentials) |
| `/connect/userinfo` | UserInfo endpoint |
| `/health/ready` | Readiness health probe |

## 6. Logging

Serilog is configured to output structured JSON to the console. Adjust `Serilog` settings in configuration to log elsewhere.

## 7. Uninstall / Clean

1. Stop the running process.
2. Remove the database if desired.
3. Delete the repository directory.

---

Next: [Configuration](CONFIGURATION.md) · [Security](SECURITY.md) · [Troubleshooting](TROUBLESHOOTING.md)
