namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// Minimal stand-in for ASP.NET Core IdentityRole.
/// </summary>
public class IdentityRole<TKey> where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; set; } = default!;
    public virtual string? Name { get; set; }
    public virtual string? NormalizedName { get; set; }
    public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
}
