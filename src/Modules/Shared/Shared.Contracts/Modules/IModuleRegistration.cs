using Microsoft.Extensions.DependencyInjection;

namespace coaches.Modules.Shared.Contracts.Modules;

public interface IModuleRegistration
{
   IServiceCollection RegisterModule(IServiceCollection services);
}
