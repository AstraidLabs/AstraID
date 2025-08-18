using AstraID.Application.Users.Commands.RegisterUser;
using AstraID.Application.Users.Queries.GetUserById;
using AstraID.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AstraID.Api.Extensions;

/// <summary>
/// Maps user-related application endpoints.
/// </summary>
public static class ApplicationEndpointsUsers
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users").WithTags("Users");

        group.MapPost("/", async (RegisterUserCommand command, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            if (!result.IsSuccess)
                return result.ToProblemDetails(ctx);
            return Results.Created($"/api/users/{result.Value}", null);
        }).RequireAuthorization("require.users.write");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetUserByIdQuery(id), ct);
            if (!result.IsSuccess)
                return result.ToProblemDetails(ctx);
            return Results.Ok(result.Value);
        }).RequireAuthorization(policy =>
            policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") || ctx.User.HasClaim("scope", "users.read")));

        return endpoints;
    }
}
