using Azure.Identity;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Web.Middlewares;
using CleanArchitecture.Web.Services;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IUser, CurrentUser>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddProblemDetails();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();
        services.AddSwagger(builder);

        return services;
    }

    public static IServiceCollection AddKeyVaultIfConfigured(this IServiceCollection services, ConfigurationManager configuration)
    {
        var keyVaultUri = configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }

        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = $"{builder.Environment.ApplicationName} v1", Version = "v1" });
            options.SwaggerDoc("v2", new() { Title = $"{builder.Environment.ApplicationName} v2", Version = "v2" });
        });
        return services;
    }

    public static IHostBuilder AddHosts(this IHostBuilder builder)
    {
        builder.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.OpenTelemetry();
        });

        builder.ConfigureServices((context, services) =>
        {
            services.AddOpenTelemetry()
                .WithTracing(tracerProvider =>
                {
                    tracerProvider
                        .AddSource(context.HostingEnvironment.ApplicationName)
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(serviceName: context.HostingEnvironment.ApplicationName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                })
                .WithMetrics(meterProvider =>
                {
                    meterProvider
                        .AddMeter(context.HostingEnvironment.ApplicationName)
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(serviceName: context.HostingEnvironment.ApplicationName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                });
        });

        return builder;
    }
}
