using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

// This is what Program.cs calls — keeps wiring clean
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // MediatR scans ALL handlers in Application assembly automatically
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register Pipeline Behaviours — ORDER MATTERS

        // ValidationBehaviour runs second (inner)


        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>));

        // Register all validators in Application assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}