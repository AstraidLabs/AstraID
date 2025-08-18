using System.Linq;
using System.Threading.Tasks;
using AstraID.Api.DTOs.Admin;
using AstraID.Api.Infrastructure.Audit;
using AstraID.Api.Security;
using AstraID.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AstraID.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/roles")]
[Authorize(Policy = Policies.Admin)]
public class RolesController : ControllerBase
{
    private readonly RoleManager<AppRole> _roles;
    private readonly UserManager<AppUser> _users;
    private readonly IAuditLogger _audit;

    public RolesController(RoleManager<AppRole> roles, UserManager<AppUser> users, IAuditLogger audit)
    {
        _roles = roles;
        _users = users;
        _audit = audit;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_roles.Roles.Select(r => new RoleDto(r.Name!)));

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        var result = await _roles.CreateAsync(new AppRole { Name = dto.Name });
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        _audit.Log("CreateRole", dto.Name);
        return Created($"/api/admin/roles/{dto.Name}", null);
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        var role = await _roles.FindByNameAsync(name);
        if (role is null) return NotFound();
        var users = await _users.GetUsersInRoleAsync(name);
        if (users.Any())
            return BadRequest(new { error = "Role in use" });
        var result = await _roles.DeleteAsync(role);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        _audit.Log("DeleteRole", name);
        return NoContent();
    }
}
