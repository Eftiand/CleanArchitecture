using System.Diagnostics.CodeAnalysis;
using coaches.Modules.Shared.Contracts.Common.Extensions;

namespace coaches.Web.Infrastructure;

public static class IEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        return MapRoute(builder, handler, HttpMethod.Get, pattern);
    }

    public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        return MapRoute(builder, handler, HttpMethod.Post, pattern);
    }

    public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern)
    {
        return MapRoute(builder, handler, HttpMethod.Put, pattern);
    }

    public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern)
    {
        return MapRoute(builder, handler, HttpMethod.Delete, pattern);
    }

    public static IEndpointRouteBuilder MapRoute(this IEndpointRouteBuilder builder, Delegate handler, HttpMethod method, [StringSyntax("Route")] string pattern)
    {
        Guard.Against.AnonymousMethod(handler);
        string methodName = handler.Method.Name.ToKebabCase();

        builder.MapMethods(pattern.ToKebabCase(), [method.ToString()], handler)
            .WithName(methodName);

        return builder;
    }
}
