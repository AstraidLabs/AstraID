using Microsoft.EntityFrameworkCore;

namespace AstraID.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void UseProviderConventions(this ModelBuilder modelBuilder, string provider)
    {
        // provider-specific conventions can be added here
    }
}
