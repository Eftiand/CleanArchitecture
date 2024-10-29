using System.Reflection;
using Shared.Const;
using Shared.Extensions;

namespace CleanArchitecture.Web.Infrastructure;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group, string version = CommonConstants.Api.Version.V1)
    {
        var groupName = group.GetType().Name.ToKebabCase();

        return app
            .MapGroup($"api/{version}/{groupName}")
            .WithGroupName(version)
            .WithTags(groupName);
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);
        var assembly = Assembly.GetExecutingAssembly();
        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
            else
            {
                Console.WriteLine($"Failed to instantiate: {type.Name}");
            }
        }

        return app;
    }
}
