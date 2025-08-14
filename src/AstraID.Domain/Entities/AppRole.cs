using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Domain.Entities;

/// <summary>
/// Application role entity.
/// </summary>
[Table("Roles", Schema = "auth")]
[Index(nameof(NormalizedName))]
public sealed class AppRole : IdentityRole<Guid>
{
    /// <summary>
    /// Optional description for the role.
    /// </summary>
    [MaxLength(256)]
    public string? Description { get; private set; }

    [ConcurrencyCheck]
    public override string? ConcurrencyStamp { get; set; }

    private AppRole()
    {
    }

    private AppRole(string name, string? description)
    {
        Id = Guid.NewGuid();
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    public static AppRole Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        var trimmed = name.Trim();
        return new AppRole(trimmed, description?.Trim());
    }
}
