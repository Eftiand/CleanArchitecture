using System.Reflection;
using coaches.Modules.Shared.Contracts.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace coaches.Modules.Todos.Application;

public static class TodosModule
{
    public static IServiceCollection AddTodoModules(this IServiceCollection services)
    {
        services.RegisterModule(Assembly.GetExecutingAssembly());
        return services;
    }
}
