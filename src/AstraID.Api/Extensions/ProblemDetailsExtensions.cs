using System.Linq;
using AstraID.Application.Common;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AstraID.Api.Extensions;

/// <summary>
/// Configures rich ProblemDetails responses and helpers for converting failures.
/// </summary>
public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddAstraIdProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(opts =>
        {
            opts.IncludeExceptionDetails = (ctx, ex) => false;
            opts.Map<ValidationException>((ctx, ex) =>
            {
                var pd = new ProblemDetails
                {
                    Title = "Validation error",
                    Status = StatusCodes.Status422UnprocessableEntity
                };
                pd.Extensions["errors"] = ex.Errors
                    .Select(e => new { field = e.PropertyName, error = e.ErrorMessage });
                pd.Extensions["traceId"] = ctx.TraceIdentifier;
                return pd;
            });
            opts.Map<UnauthorizedAccessException>((ctx, ex) => new ProblemDetails
            {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Extensions = { ["traceId"] = ctx.TraceIdentifier }
            });
            opts.Map<Exception>((ctx, ex) => new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Extensions = { ["traceId"] = ctx.TraceIdentifier }
            });
        });
        return services;
    }

    public static IResult ToProblemDetails(this Result result, HttpContext ctx)
    {
        var pd = new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = result.ErrorMessage
        };
        if (!string.IsNullOrWhiteSpace(result.ErrorCode))
            pd.Extensions["code"] = result.ErrorCode;
        pd.Extensions["traceId"] = ctx.TraceIdentifier;
        return Results.Json(pd, statusCode: pd.Status);
    }

    public static IResult ToProblemDetails<T>(this Result<T> result, HttpContext ctx)
        => ToProblemDetails(Result.Failure(result.ErrorCode!, result.ErrorMessage!), ctx);
}
