using System;
using System.Linq;
using System.Threading.Tasks;
using AstraID.Api.DTOs.Admin;
using AstraID.Api.Infrastructure.Audit;
using AstraID.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace AstraID.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/clients")]
[Authorize(Policy = Policies.Admin)]
public class ClientsController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _apps;
    private readonly IAuditLogger _audit;

    public ClientsController(IOpenIddictApplicationManager apps, IAuditLogger audit)
    {
        _apps = apps;
        _audit = audit;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var list = new System.Collections.Generic.List<ClientDto>();
        await foreach (var app in _apps.ListAsync())
        {
            var descriptor = await _apps.GetDescriptorAsync(app);
            var id = Guid.Parse(await _apps.GetIdAsync(app)!);
            var clientId = await _apps.GetClientIdAsync(app)!;
            list.Add(new ClientDto(id, clientId, descriptor.DisplayName, descriptor.RedirectUris.Select(u => u.ToString()), descriptor.Permissions));
        }
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var app = await _apps.FindByIdAsync(id.ToString());
        if (app == null) return NotFound();
        var descriptor = await _apps.GetDescriptorAsync(app);
        return Ok(new ClientDto(Guid.Parse(await _apps.GetIdAsync(app)!), await _apps.GetClientIdAsync(app)!, descriptor.DisplayName, descriptor.RedirectUris.Select(u => u.ToString()), descriptor.Permissions));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClientDto dto)
    {
        if (await _apps.FindByClientIdAsync(dto.ClientId) != null)
            return BadRequest(new { error = "duplicate_client" });
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = dto.ClientId,
            ClientSecret = dto.ClientSecret,
            DisplayName = dto.DisplayName
        };
        foreach (var uri in dto.RedirectUris)
            descriptor.RedirectUris.Add(new Uri(uri));
        foreach (var p in dto.Permissions)
            descriptor.Permissions.Add(p);
        var app = await _apps.CreateAsync(descriptor);
        var id = await _apps.GetIdAsync(app);
        _audit.Log("CreateClient", id!);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateClientDto dto)
    {
        var app = await _apps.FindByIdAsync(id.ToString());
        if (app == null) return NotFound();
        var descriptor = await _apps.GetDescriptorAsync(app);
        descriptor.DisplayName = dto.DisplayName;
        descriptor.ClientSecret = dto.ClientSecret;
        descriptor.RedirectUris.Clear();
        foreach (var uri in dto.RedirectUris)
            descriptor.RedirectUris.Add(new Uri(uri));
        descriptor.Permissions.Clear();
        foreach (var p in dto.Permissions)
            descriptor.Permissions.Add(p);
        await _apps.UpdateAsync(app, descriptor);
        _audit.Log("UpdateClient", id.ToString());
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var app = await _apps.FindByIdAsync(id.ToString());
        if (app == null) return NotFound();
        await _apps.DeleteAsync(app);
        _audit.Log("DeleteClient", id.ToString());
        return NoContent();
    }
}
