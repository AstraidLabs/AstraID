using AstraID.Domain.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstraID.Domain.Entities;

/// <summary>
/// Join entity between roles and permissions.
/// </summary>
[Table("RolePermissions", Schema = "auth")]
public sealed class RolePermission : Entity<Guid>
{
    /// <summary>
    /// Role identifier.
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// Permission identifier.
    /// </summary>
    public int PermissionId { get; private set; }

    private RolePermission()
    {
    }

    private RolePermission(Guid roleId, int permissionId)
    {
        Id = Guid.NewGuid();
        RoleId = roleId;
        PermissionId = permissionId;
    }

    /// <summary>
    /// Creates a new association between a role and permission.
    /// </summary>
    public static RolePermission Create(Guid roleId, int permissionId)
        => new(roleId, permissionId);
}
