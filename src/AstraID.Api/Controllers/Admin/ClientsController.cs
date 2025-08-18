using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        var list = new List<ClientDto>();

        await foreach (var app in _apps.ListAsync())
        {
            var idStr = await _apps.GetIdAsync(app);
            var clientId = await _apps.GetClientIdAsync(app);
            var displayName = await _apps.GetDisplayNameAsync(app);
            var redirects = await _apps.GetRedirectUrisAsync(app);
            var perms = await _apps.GetPermissionsAsync(app);

            if (!Guid.TryParse(idStr, out var id))
                continue; // nebo BadData – podle tvého modelu

            list.Add(new ClientDto(
                id,
                clientId!,
                displayName,
                redirects.Select(u => u.ToString()),
                perms
            ));
        }

        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var app = await _apps.FindByIdAsync(id.ToString());
        if (app is null) return NotFound();

        var clientId = await _apps.GetClientIdAsync(app);
        var displayName = await _apps.GetDisplayNameAsync(app);
        var redirects = await _apps.GetRedirectUrisAsync(app);
        var perms = await _apps.GetPermissionsAsync(app);

        return Ok(new ClientDto(
            id,
            clientId!,
            displayName,
            redirects.Select(u => u.ToString()),
            perms
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
    {
        if (await _apps.FindByClientIdAsync(dto.ClientId) is not null)
            return BadRequest(new { error = "duplicate_client" });

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = dto.ClientId,
            ClientSecret = dto.ClientSecret, // OpenIddict si tajemství pøi persistenci sám zahashuje
            DisplayName = dto.DisplayName
        };

        foreach (var uri in dto.RedirectUris ?? Enumerable.Empty<string>())
            descriptor.RedirectUris.Add(new Uri(uri));

        foreach (var p in dto.Permissions ?? Enumerable.Empty<string>())
            descriptor.Permissions.Add(p);

        var app = await _apps.CreateAsync(descriptor);
        var id = await _apps.GetIdAsync(app);

        _audit.Log("CreateClient", id!);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientDto dto)
    {
        var app = await _apps.FindByIdAsync(id.ToString());
        if (app is null) return NotFound();

        // Naèti aktuální stav do descriptoru a uprav jen zmìny
        var descriptor = new OpenIddictApplicationDescriptor();
        await _apps.PopulateAsync(descriptor, app);

        descriptor.DisplayName = dto.DisplayName;
        descriptor.ClientSecret = dto.ClientSecret; // pokud null, ponech stávající (mùžeš podmínit)

        descriptor.RedirectUris.Clear();
        foreach (var uri in dto.RedirectUris ?? Enumerable.Empty<string>())
            descriptor.RedirectUris.Add(new Uri(uri));

        descriptor.Permissions.Clear();
        foreach (var p in dto.Permissions ?? Enumerable.Empty<string>())
            descriptor.Permissions.Add(p);

        await _apps.UpdateAsync(app, descriptor);
        _audit.Log("UpdateClient", id.ToString());
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var app = await _apps.FindByIdAsync(id.ToString());
        if (app is null) return NotFound();

        await _apps.DeleteAsync(app);
        _audit.Log("DeleteClient", id.ToString());
        return NoContent();
    }
}
