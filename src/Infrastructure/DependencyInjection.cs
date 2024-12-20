﻿using System.Reflection;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Data.Interceptors;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Messaging;
using CleanArchitecture.Infrastructure.Messaging.Filters;
using CleanArchitecture.Shared.Contracts.Messaging;
using Hangfire;
using Hangfire.PostgreSql;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Const;

namespace CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(CommonConstants.Aspire.Postgres);

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseNpgsql(connectionString);
        });


        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddMapster();

        services.AddAuthorizationBuilder();

        services.AddCustomMassTransit(configuration, config =>
        {
            config.AddConsumers(Assembly.Load(CommonConstants.Assemblies.Application));
        });

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<ISender, Sender>();

        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        services.AddHangfireImplementation(configuration);

        return services;
    }

    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator>? configure = null)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            // Allow additional configuration before RabbitMQ setup
            configure?.Invoke(config);

            config.UsingRabbitMq((context, cfg) =>
            {
                PipelineConfig.ConfigurePipeline(cfg, context);

                cfg.Host(configuration.GetConnectionString(CommonConstants.Aspire.RabbitMq), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollection AddHangfireImplementation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(configuration.GetConnectionString(CommonConstants.Aspire.Postgres));
            })
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings());

        services.AddHangfireServer();

        return services;
    }

}

public static class PipelineConfig
{
    public static readonly Action<IBusFactoryConfigurator, IRegistrationContext> ConfigurePipeline = (cfg, context) =>
    {
        cfg.UseConsumeFilter(typeof(UnhandledExceptionFilter<>), context);
        cfg.UseConsumeFilter(typeof(LoggingFilter<>), context);
        cfg.UseConsumeFilter(typeof(PerformanceFilter<>), context);
        cfg.UseConsumeFilter(typeof(AuthorizationFilter<>), context);
        cfg.UseConsumeFilter(typeof(ValidationFilter<>), context);
        cfg.UseConsumeFilter(typeof(UnitOfWorkFilter<>), context);
    };
}
