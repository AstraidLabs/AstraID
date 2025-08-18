using System;
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
[Route("api/admin/users")]
[Authorize(Policy = Policies.Admin)]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _users;
    private readonly IAuditLogger _audit;

    public UsersController(UserManager<AppUser> users, IAuditLogger audit)
    {
        _users = users;
        _audit = audit;
    }

    [HttpGet]
    public IActionResult GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var query = _users.Users;
        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => u.Email!.Contains(search));
        var items = query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(u => new UserDto(u.Id, u.Email, Array.Empty<string>()));
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _users.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();
        var roles = await _users.GetRolesAsync(user);
        return Ok(new UserDto(user.Id, user.Email, roles));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        var user = new AppUser { UserName = dto.Email, Email = dto.Email };
        var result = await _users.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        _audit.Log("CreateUser", user.Id.ToString());
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new UserDto(user.Id, user.Email, Array.Empty<string>()));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserDto dto)
    {
        var user = await _users.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();
        if (!string.IsNullOrEmpty(dto.Email))
            user.Email = user.UserName = dto.Email;
        var result = await _users.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        if (dto.Roles != null)
        {
            var current = await _users.GetRolesAsync(user);
            await _users.RemoveFromRolesAsync(user, current);
            await _users.AddToRolesAsync(user, dto.Roles);
        }
        _audit.Log("UpdateUser", id.ToString());
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _users.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();
        var result = await _users.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        _audit.Log("DeleteUser", id.ToString());
        return NoContent();
    }
}
