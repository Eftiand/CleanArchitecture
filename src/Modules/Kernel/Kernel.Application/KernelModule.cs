using System.Reflection;
using coaches.Modules.Shared.Contracts.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace coaches.Modules.Kernel.Application;

public static class KernelModule
{
    public static IServiceCollection AddKernelModule(this IServiceCollection services)
    {
        services.RegisterModule(Assembly.GetExecutingAssembly());
        return services;
    }
}
