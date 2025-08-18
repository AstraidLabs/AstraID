using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AstraID.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace AstraID.Api.Controllers;

/// <summary>
/// Implements the OIDC userinfo endpoint.
/// </summary>
[ApiController]
[Route("connect/userinfo")]
[Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
public class UserInfoController : ControllerBase
{
    private readonly UserManager<AppUser> _users;

    public UserInfoController(UserManager<AppUser> users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
        var subject = User.FindFirstValue(OpenIddictConstants.Claims.Subject);
        if (subject is null)
            return Challenge();

        var user = await _users.FindByIdAsync(subject);
        if (user is null)
            return Challenge();

        var scopes = User.GetScopes();
        var claims = new Dictionary<string, object?>
        {
            [OpenIddictConstants.Claims.Subject] = subject
        };

        if (scopes.Contains(OpenIddictConstants.Scopes.Profile))
        {
            claims[OpenIddictConstants.Claims.Name] = user.UserName;
        }
        if (scopes.Contains(OpenIddictConstants.Scopes.Email))
        {
            claims[OpenIddictConstants.Claims.Email] = user.Email;
            claims[OpenIddictConstants.Claims.EmailVerified] = user.EmailConfirmed;
        }
        if (scopes.Contains("roles"))
        {
            var roles = await _users.GetRolesAsync(user);
            claims["roles"] = roles.ToArray();
        }
        return Ok(claims);
    }
}
