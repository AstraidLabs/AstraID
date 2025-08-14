using AstraID.Domain.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents an application permission.
/// </summary>
[Table("Permissions", Schema = "auth")]
[Index(nameof(Name), IsUnique = true)]
public sealed class Permission : Entity<int>
{
    /// <summary>
    /// Permission name (unique).
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Optional description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Indicates if the permission is built-in and immutable.
    /// </summary>
    public bool IsBuiltIn { get; private set; }

    private Permission()
    {
    }

    private Permission(string name, string? description, bool isBuiltIn)
    {
        Name = name;
        Description = description;
        IsBuiltIn = isBuiltIn;
    }

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    public static Permission Create(int id, string name, string? description = null, bool isBuiltIn = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty.", nameof(name));

        var trimmed = name.Trim();
        return new Permission(trimmed, description?.Trim(), isBuiltIn) { Id = id };
    }

    /// <summary>
    /// Throws if the permission is built-in.
    /// </summary>
    public void EnsureCanDelete()
    {
        if (IsBuiltIn)
            throw new InvalidOperationException("Built-in permissions cannot be deleted.");
    }
}
