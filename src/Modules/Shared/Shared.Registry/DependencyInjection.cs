using System.Reflection;
using coaches.Modules.Kernel.Application;
using coaches.Modules.Todos.Application;
using Microsoft.Extensions.DependencyInjection;

namespace coaches.Modules.Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddModules();

        return services;
    }

    public static IServiceCollection AddModules(this IServiceCollection services)
    {
        services.AddTodoModules();
        services.AddKernelModule();

        return services;
    }
}
