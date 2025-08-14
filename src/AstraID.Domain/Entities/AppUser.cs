using Microsoft.AspNetCore.Identity;

namespace AstraID.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; }
    public DateTime? LastLoginUtc { get; set; }
}
