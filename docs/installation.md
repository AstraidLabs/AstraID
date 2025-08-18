# Installation and Configuration

This guide explains how to set up **AstraID** for local development or deployment.

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/) or Docker
- Git

## Clone the Repository
```bash
git clone https://github.com/<your-org>/AstraID.git
cd AstraID
```

## Running with the .NET SDK
1. Configure required environment variables:
   ```bash
   export ASTRAID_DB_PROVIDER=SqlServer
   export ASTRAID_DB_CONN="<connection-string>"
   export ASTRAID_ALLOWED_CORS="https://app.example.com"
   export ASTRAID_ISSUER="https://auth.example.com"
   export ASTRAID_OUTBOX__POLL_INTERVAL_SECONDS=5
   ```
   Optional development helpers:
   ```bash
   export ASTRAID_AUTO_MIGRATE=true
   export ASTRAID_RUN_SEED=true
   export ASTRAID_ADMIN_EMAIL="admin@example.com"
   export ASTRAID_ADMIN_PASSWORD="ChangeMe!"
   export ASTRAID_ADMIN_ROLES="Admin"
   export ASTRAID_ADMIN_CLIENT_ID="astraid-cli"
   export ASTRAID_ADMIN_CLIENT_SECRET="secret"
   export ASTRAID_WEB_CLIENT_ID="web"
   export ASTRAID_WEB_REDIRECTS="https://app.example.com/signin-callback"
   export ASTRAID_SCOPES="api"
   ```
2. Run the API:
   ```bash
   dotnet run --project src/AstraID.Api
   ```
   The service listens on `http://localhost:8080`. Health probes are exposed at `/health/live` and `/health/ready`.

## Running with Docker
Build and run a container image:
```bash
docker build -f deploy/Dockerfile -t astraid-server .
docker run -p 8080:8080 -e ASTRAID_DB_CONN="..." astraid-server
```
Using Docker Compose:
```bash
cd deploy
docker compose up --build
```

## Configuration Reference
| Variable | Description | Default |
| --- | --- | --- |
| `ASTRAID_DB_PROVIDER` | Database provider (`SqlServer` or `Sqlite`) | `SqlServer` |
| `ASTRAID_DB_CONN` | Database connection string | — |
| `ASTRAID_ALLOWED_CORS` | Comma-separated allowed origins | — |
| `ASTRAID_ISSUER` | Issuer URI used for tokens | — |
| `ASTRAID_OUTBOX__POLL_INTERVAL_SECONDS` | Background outbox polling interval | `5` |
| `ASTRAID_AUTO_MIGRATE` | Apply EF Core migrations on startup | `false` |
| `ASTRAID_RUN_SEED` | Seed sample data on startup | `false` |
| `ASTRAID_ADMIN_EMAIL`, `ASTRAID_ADMIN_PASSWORD` | Initial admin user credentials | — |
| `ASTRAID_ADMIN_ROLES` | Roles assigned to the admin user | — |
| `ASTRAID_SCOPES` | OAuth scopes created during seeding | — |
| `ASTRAID_ADMIN_CLIENT_ID`, `ASTRAID_ADMIN_CLIENT_SECRET` | Confidential client credentials | — |
| `ASTRAID_WEB_CLIENT_ID`, `ASTRAID_WEB_REDIRECTS` | Public web client configuration | — |

For additional configuration options, review the source in `src/AstraID.Infrastructure` and `src/AstraID.Persistence`.
