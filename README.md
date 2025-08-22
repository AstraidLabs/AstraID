# AstraID – authorization & authentication server (ASP.NET Core 9 + OpenIddict 7)

[![Build](https://github.com/AstraID/AstraID/actions/workflows/build.yml/badge.svg)](https://github.com/AstraID/AstraID/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/AstraID/AstraID.svg)](LICENSE)

AstraID provides OAuth2 and OpenID Connect services with a config‑first approach for secure token issuance.

## Features
- ASP.NET Core 9, EF Core 9 and OpenIddict 7
- Authorization Code + PKCE, Refresh Token and Client Credentials flows
- SQL Server persistence with optional Windows authentication
- Config‑driven CORS and rate limiting
- Structured logging via Serilog and optional OpenTelemetry

## Quick Start

1. **Install prerequisites**
   - .NET 9 SDK
   - SQL Server or LocalDB
   - EF Core tools: `dotnet tool install --global dotnet-ef`
   - TLS certificate for Kestrel (PFX) or rely on dev certs

2. **Clone and restore**
   ```bash
   git clone https://github.com/AstraID/AstraID.git
   cd AstraID
   dotnet restore
   ```

3. **Configure**
   ```bash
   cp docs/SAMPLES/appsettings.Local.json.example src/AstraID.Api/appsettings.Local.json
   # edit connection string, AllowedCors, issuer and other values
   ```

4. **Create or update the database**
   ```bash
   dotnet ef database update -p src/AstraID.Persistence -s src/AstraID.Api
   ```
   Run this after adding new migrations as well.

5. **Run**
   ```bash
   dotnet run -p src/AstraID.Api --launch-profile https
   ```

6. **Verify**
   - Browse to `https://localhost:5001/.well-known/openid-configuration`
   - Check health at `https://localhost:5001/health/ready`

## How to run tests

1. Ensure the .NET 9 SDK is installed.
2. Restore and build the solution:
   ```bash
   dotnet restore
   dotnet build
   ```
3. Run all tests with coverage:
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```
   A local SQL Server instance is required only for tests that need it; the default configuration uses in-memory databases.

## Documentation
- [Installation Guide](docs/INSTALL.md)
- [Configuration Reference](docs/CONFIGURATION.md)
- [Security Notes](docs/SECURITY.md)
- [Troubleshooting](docs/TROUBLESHOOTING.md)
- [Upgrade Guide](docs/UPGRADE.md)
- [Test Plan](AstraID_TestPlan.md)
- [Action Plan](AstraID_FixPlan.md)
- [Readiness Report](AstraID_ReadinessChecklist.md)

---

Return to [Installation](docs/INSTALL.md) · [Configuration](docs/CONFIGURATION.md)

## License
Licensed under the terms of the project [LICENSE](LICENSE).
