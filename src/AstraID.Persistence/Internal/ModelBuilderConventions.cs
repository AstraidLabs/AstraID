using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AstraID.Persistence.Internal;

internal static class ModelBuilderConventions
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(string)))
        {
            if (property.GetMaxLength() is null)
            {
                property.SetMaxLength(256);
            }

            if (property.GetColumnType() is null)
            {
                var length = property.GetMaxLength();
                property.SetColumnType(length is null ? "nvarchar(max)" : $"nvarchar({length})");
            }
        }
    }
}
