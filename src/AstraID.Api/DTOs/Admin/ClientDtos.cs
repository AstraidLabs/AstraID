using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AstraID.Api.DTOs.Admin;

public record ClientDto(Guid Id, string ClientId, string? DisplayName, IEnumerable<string> RedirectUris, IEnumerable<string> Permissions);

public class CreateClientDto
{
    [Required]
    public string ClientId { get; set; } = default!;
    public string? DisplayName { get; set; }
    public string? ClientSecret { get; set; }
    public IEnumerable<string> RedirectUris { get; set; } = Array.Empty<string>();
    public IEnumerable<string> Permissions { get; set; } = Array.Empty<string>();
}

public class UpdateClientDto : CreateClientDto { }
