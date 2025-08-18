using System.ComponentModel.DataAnnotations;

namespace AstraID.Api.DTOs.Admin;

public record RoleDto(string Name);

public class CreateRoleDto
{
    [Required]
    public string Name { get; set; } = default!;
}
