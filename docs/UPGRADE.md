# Upgrade Guide

Use this guide to upgrade AstraID between versions. Always back up your database and configuration files before proceeding.

## 0.2 → 0.3

| Old Setting | New Setting | Action |
|-------------|-------------|--------|
| `Auth:TokenFormat` | `Auth:AccessTokenFormat` | Rename key and set to `Jwt` or `Reference`. |
| `AstraId:RequirePar` | `AstraId:RequireParForPublicClients` | Rename key if enabling PAR for public clients. |

### Steps

1. Update configuration files to use the new keys shown above.
2. Review [CONFIGURATION.md](CONFIGURATION.md) for new options.
3. Apply any pending EF Core migrations:
   ```bash
   dotnet ef database update -p src/AstraID.Persistence -s src/AstraID.Api
   ```
4. Rebuild and run the API.

## EF Core Schema Changes

AstraID does **not** apply schema updates automatically unless `AstraId:AutoMigrate=true`.
For production environments:

1. Generate migrations in source control.
2. Review SQL scripts before applying to production.
3. Apply migrations using the command above.

## Troubleshooting

If startup fails after upgrading, check `docs/TROUBLESHOOTING.md` and verify that configuration keys match the current version.
