using System;
using System.Linq;

namespace AstraID.Api.Options;

internal static class ValidationError
{
    public static string Format(string key, string message, string fix)
        => string.Join('|', new[]{key, message, fix});

    public static (string Key, string Message, string Fix) Parse(string value)
    {
        var parts = value.Split('|', 3);
        return (
            parts.ElementAtOrDefault(0) ?? string.Empty,
            parts.ElementAtOrDefault(1) ?? string.Empty,
            parts.ElementAtOrDefault(2) ?? string.Empty
        );
    }
}
