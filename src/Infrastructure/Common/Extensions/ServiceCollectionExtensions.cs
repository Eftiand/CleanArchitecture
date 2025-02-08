using Microsoft.Extensions.DependencyInjection;

namespace coaches.Infrastructure.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RemoveServicesFromNamespace(this IServiceCollection services, string @namespace)
    {
        var descriptors = services
            .Where(d => d.ServiceType.Namespace?.StartsWith(@namespace) == true ||
                        d.ImplementationType?.Namespace?.StartsWith(@namespace) == true)
            .ToList();

        descriptors.ForEach(d => services.Remove(d));

        return services;
    }
}
