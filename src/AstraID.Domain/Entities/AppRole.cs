using Microsoft.AspNetCore.Identity;

namespace AstraID.Domain.Entities;

public class AppRole : IdentityRole<Guid>
{
    public AppRole() : base()
    {
    }

    public AppRole(string roleName) : base(roleName)
    {
    }
}
