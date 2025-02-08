using System.Reflection;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace coaches.Modules.Shared.Contracts.Modules;

public static class ModuleExtensions
{
    public static IServiceCollection RegisterModule(this IServiceCollection services, Assembly assembly)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumers(assembly);
        });
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
