using AstraID.Domain.Entities;
using AstraID.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AstraID.Infrastructure.Startup;

/// <summary>
/// Provides optional database migration and seeding for AstraID.
/// Both features are disabled by default and should be enabled via
/// environment variables for development or testing only. In production,
/// prefer running migrations and seeding as part of CI/CD or one-off jobs.
/// </summary>
public static class WebAppDatabaseExtensions
{
    public static async Task UseAstraIdDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("AstraID.Startup");
        var db = services.GetRequiredService<AstraIdDbContext>();

        var shouldMigrate = configuration.GetValue("ASTRAID_AUTO_MIGRATE", false);
        var shouldSeed = configuration.GetValue("ASTRAID_RUN_SEED", false);

        if (shouldMigrate)
        {
            try
            {
                logger.LogInformation("Applying database migrations...");
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration failed");
                throw;
            }
        }
        else
        {
            logger.LogInformation("ASTRAID_AUTO_MIGRATE=false. Skipping migrations.");
        }

        if (shouldSeed)
        {
            try
            {
                var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var scopeManager = services.GetRequiredService<IOpenIddictScopeManager>();
                var appManager = services.GetRequiredService<IOpenIddictApplicationManager>();
                var options = SeedOptions.FromConfiguration(configuration);
                await SeedAsync(roleManager, userManager, scopeManager, appManager, logger, options, app.Lifetime.ApplicationStopping);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database seeding failed");
                throw;
            }
        }
        else
        {
            logger.LogInformation("ASTRAID_RUN_SEED=false. Skipping seeding.");
        }
    }

    private static async Task SeedAsync(
        RoleManager<AppRole> roles,
        UserManager<AppUser> users,
        IOpenIddictScopeManager scopeManager,
        IOpenIddictApplicationManager appManager,
        ILogger logger,
        SeedOptions options,
        CancellationToken ct)
    {
        // Roles
        foreach (var roleName in options.AdminRoles)
        {
            if (await roles.RoleExistsAsync(roleName))
            {
                logger.LogInformation("Role {Role} already exists", roleName);
                continue;
            }

            var role = AppRole.Create(roleName, "Seeded role");
            var result = await roles.CreateAsync(role);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            logger.LogInformation("Created role {Role}", roleName);
        }

        // Admin user
        if (!string.IsNullOrWhiteSpace(options.AdminEmail) && !string.IsNullOrWhiteSpace(options.AdminPassword))
        {
            var admin = await users.FindByEmailAsync(options.AdminEmail);
            if (admin is null)
            {
                var email = Domain.ValueObjects.Email.Create(options.AdminEmail);
                var display = Domain.ValueObjects.DisplayName.Create(options.AdminEmail);
                admin = Domain.Entities.AppUser.Register(email, display);
                admin.ConfirmEmail();
                var createResult = await users.CreateAsync(admin, options.AdminPassword);
                if (!createResult.Succeeded)
                    throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                logger.LogInformation("Created admin user {Email}", options.AdminEmail);
            }
            else
            {
                logger.LogInformation("Admin user {Email} already exists", options.AdminEmail);
            }

            var missingRoles = new List<string>();
            foreach (var role in options.AdminRoles)
            {
                if (!await users.IsInRoleAsync(admin, role))
                {
                    missingRoles.Add(role);
                }
            }
            if (missingRoles.Count > 0)
            {
                var addResult = await users.AddToRolesAsync(admin, missingRoles);
                if (!addResult.Succeeded)
                    throw new InvalidOperationException($"Failed to assign roles to admin user: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
                logger.LogInformation("Assigned roles {Roles} to admin user {Email}", string.Join(',', missingRoles), options.AdminEmail);
            }
        }
        else
        {
            logger.LogWarning("Admin user seeding skipped. ASTRAID_ADMIN_EMAIL or ASTRAID_ADMIN_PASSWORD not provided.");
        }

        // Scopes
        foreach (var scope in options.Scopes)
        {
            if (await scopeManager.FindByNameAsync(scope, ct) is null)
            {
                var descriptor = new OpenIddictScopeDescriptor
                {
                    Name = scope,
                    DisplayName = scope
                };
                await scopeManager.CreateAsync(descriptor, ct);
                logger.LogInformation("Created scope {Scope}", scope);
            }
            else
            {
                logger.LogInformation("Scope {Scope} already exists", scope);
            }
        }

        // Admin client (CLI)
        if (!string.IsNullOrWhiteSpace(options.AdminClientId))
        {
            if (await appManager.FindByClientIdAsync(options.AdminClientId, ct) is null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = options.AdminClientId,
                    ClientType = ClientTypes.Confidential,
                    ClientSecret = options.AdminClientSecret,
                    DisplayName = options.AdminClientId
                };
                descriptor.Permissions.Add(Permissions.Endpoints.Token);
                descriptor.Permissions.Add(Permissions.GrantTypes.ClientCredentials);
                foreach (var scope in options.Scopes)
                {
                    descriptor.Permissions.Add(Permissions.Prefixes.Scope + scope);
                }
                await appManager.CreateAsync(descriptor, ct);
                logger.LogInformation("Created admin client {ClientId}", options.AdminClientId);
            }
            else
            {
                logger.LogInformation("Admin client {ClientId} already exists", options.AdminClientId);
            }
        }
        else
        {
            logger.LogWarning("Admin client seeding skipped. ASTRAID_ADMIN_CLIENT_ID not provided.");
        }

        // Web SPA client
        if (!string.IsNullOrWhiteSpace(options.WebClientId) && options.WebRedirects.Count > 0)
        {
            if (await appManager.FindByClientIdAsync(options.WebClientId, ct) is null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = options.WebClientId,
                    ClientType = ClientTypes.Public,
                    DisplayName = options.WebClientId
                };
                descriptor.Permissions.Add(Permissions.Endpoints.Authorization);
                descriptor.Permissions.Add(Permissions.Endpoints.Token);
                descriptor.Permissions.Add(Permissions.Endpoints.EndSession);
                descriptor.Permissions.Add(Permissions.GrantTypes.AuthorizationCode);
                descriptor.Permissions.Add(Permissions.ResponseTypes.Code);
                descriptor.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
                foreach (var uri in options.WebRedirects)
                {
                    var redirect = new Uri(uri, UriKind.Absolute);
                    descriptor.RedirectUris.Add(redirect);
                    descriptor.PostLogoutRedirectUris.Add(redirect);
                }
                foreach (var scope in options.Scopes)
                {
                    descriptor.Permissions.Add(Permissions.Prefixes.Scope + scope);
                }
                await appManager.CreateAsync(descriptor, ct);
                logger.LogInformation("Created web client {ClientId}", options.WebClientId);
            }
            else
            {
                logger.LogInformation("Web client {ClientId} already exists", options.WebClientId);
            }
        }
        else
        {
            logger.LogWarning("Web client seeding skipped. ASTRAID_WEB_CLIENT_ID or ASTRAID_WEB_REDIRECTS not provided.");
        }
    }
}

