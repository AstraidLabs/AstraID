using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using AstraID.Application.Common.Mapping;
using AstraID.Application.Common.Security;
using AstraID.Application.Abstractions;

namespace AstraID.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAstraIdApplication(this IServiceCollection services)
    {
        var asm = Assembly.GetExecutingAssembly();

        services.AddMediatR(asm);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.UnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.LoggingBehavior<,>));

        services.AddValidatorsFromAssembly(asm, includeInternalTypes: true);

        var config = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Register(config);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }
}
