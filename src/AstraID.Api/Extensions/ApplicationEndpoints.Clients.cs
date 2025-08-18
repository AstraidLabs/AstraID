using AstraID.Application.Clients.Commands.RegisterClient;
using AstraID.Application.Clients.Commands.RotateClientSecret;
using AstraID.Application.Clients.Queries.GetClientById;
using AstraID.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AstraID.Api.Extensions;

/// <summary>
/// Maps client-related application endpoints.
/// </summary>
public static class ApplicationEndpointsClients
{
    public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/clients").WithTags("Clients");

        group.MapPost("/", async (RegisterClientCommand command, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            if (!result.IsSuccess)
                return result.ToProblemDetails(ctx);
            return Results.Created($"/api/clients/{result.Value}", null);
        }).RequireAuthorization("require.clients.write");

        group.MapPost("/{clientId}/rotate-secret", async (string clientId, RotateSecretRequest body, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var command = new RotateClientSecretCommand(clientId, body.NewSecret);
            var result = await mediator.Send(command, ct);
            if (!result.IsSuccess)
                return result.ToProblemDetails(ctx);
            return Results.NoContent();
        }).RequireAuthorization("require.clients.write");

        group.MapGet("/{clientId}", async (string clientId, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetClientByIdQuery(clientId), ct);
            if (!result.IsSuccess)
                return result.ToProblemDetails(ctx);
            return Results.Ok(result.Value);
        }).RequireAuthorization(policy => policy.RequireRole("Admin"));

        return endpoints;
    }

    private sealed record RotateSecretRequest(string NewSecret);
}
