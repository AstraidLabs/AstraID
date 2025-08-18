using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AstraID.Api.DTOs.Admin;

public record UserDto(Guid Id, string? Email, IEnumerable<string> Roles);

public class CreateUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required, MinLength(6)]
    public string Password { get; set; } = default!;
}

public class UpdateUserDto
{
    [EmailAddress]
    public string? Email { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}
