# AstraID – authorization & authentication server (ASP.NET Core 9 + OpenIddict 7)

[![Build](https://github.com/AstraID/AstraID/actions/workflows/build.yml/badge.svg)](https://github.com/AstraID/AstraID/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/AstraID/AstraID.svg)](LICENSE)

AstraID provides OAuth2 and OpenID Connect services with a config-first approach.

## Features
- ASP.NET Core 9, EF Core 9 and OpenIddict 7
- Authorization Code + PKCE, Refresh Token and Client Credentials flows
- SQL Server persistence with optional Windows authentication
- Structured logging via Serilog

## Quick Start

1. **Install prerequisites**
   - .NET 9 SDK
   - SQL Server or LocalDB
   - EF Core tools: `dotnet tool install --global dotnet-ef`

2. **Clone and restore**
   ```bash
   git clone https://github.com/AstraID/AstraID.git
   cd AstraID
   dotnet restore
   ```

3. **Configure**
   ```bash
   cp docs/SAMPLES/appsettings.Local.json.example src/AstraID.Api/appsettings.Local.json
   # edit connection string and other values
   ```

4. **Create the database**
   ```bash
   dotnet ef database update -p src/AstraID.Persistence -s src/AstraID.Api
   ```

5. **Run**
   ```bash
   dotnet run -p src/AstraID.Api
   ```

6. **Verify**
   - Browse to `https://localhost:5001/.well-known/openid-configuration`
   - Check health at `https://localhost:5001/health/ready`

## Documentation
- [Installation Guide](docs/INSTALL.md)
- [Configuration Reference](docs/CONFIGURATION.md)
- [Troubleshooting](docs/TROUBLESHOOTING.md)

## License
Licensed under the terms of the project [LICENSE](LICENSE).
