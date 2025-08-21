using System.Threading.Tasks;
using AstraID.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AstraID.Persistence;
using Microsoft.AspNetCore.DataProtection;

public class IdentityPoliciesTests
{
    private ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<AstraIdDbContext>(o => o.UseInMemoryDatabase("id-policy"));
        services.AddDataProtection().PersistKeysToDbContext<AstraIdDbContext>();
        services.AddIdentityCore<AppUser>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<AstraIdDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(o =>
        {
            o.Password.RequiredLength = 12;
            o.Password.RequireUppercase = true;
            o.Password.RequireLowercase = true;
            o.Password.RequireDigit = true;
            o.Password.RequireNonAlphanumeric = false;
            o.Lockout.MaxFailedAccessAttempts = 5;
            o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        });

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task WeakPassword_FailsValidation()
    {
        using var provider = BuildProvider();
        var userManager = provider.GetRequiredService<UserManager<AppUser>>();
        var user = new AppUser { UserName = "test@example.com", Email = "test@example.com" };
        var result = await userManager.CreateAsync(user, "short");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Lockout_AfterFiveFailures()
    {
        using var provider = BuildProvider();
        var userManager = provider.GetRequiredService<UserManager<AppUser>>();
        var user = new AppUser { UserName = "lock@test.com", Email = "lock@test.com" };
        await userManager.CreateAsync(user, "StrongPass123");
        for (int i = 0; i < 5; i++)
        {
            await userManager.AccessFailedAsync(user);
        }
        var locked = await userManager.IsLockedOutAsync(user);
        Assert.True(locked);
    }
}
