using System.Reflection;
using coaches.Infrastructure.Data;
using coaches.Infrastructure.Data.Interceptors;
using coaches.Infrastructure.Identity;
using coaches.Infrastructure.Messaging;
using coaches.Infrastructure.Messaging.Filters;
using coaches.Modules.Shared.Application.Common.Constants;
using coaches.Modules.Shared.Application.Common.Interfaces;
using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Modules.Shared.Contracts.Constants;
using Hangfire;
using Hangfire.Dashboard;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace coaches.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(CommonConstants.Aspire.Postgres);

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, GenericEventPublisherInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IChatClient>(_ => new OllamaChatClient(new Uri("http://localhost:11434"), "phi4"));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddMapster();

        services.AddAuthorizationBuilder();

        services.AddCustomMassTransit(configuration, config =>
        {
            config.AddConsumers(Assembly.GetExecutingAssembly());
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

        services.AddHangfireImplementation(connectionString);

        return services;
    }

    private static void AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator>? configure = null)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            // Allow additional configuration before RabbitMQ setup
            configure?.Invoke(config);

            config.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            config.UsingRabbitMq((context, cfg) =>
            {
                PipelineConfig.ConfigureMassTransitPipeline(cfg, context);

                cfg.UseMessageRetry(r => r.Interval(10, TimeSpan.FromSeconds(10)));

                cfg.Host(configuration.GetConnectionString(CommonConstants.Aspire.RabbitMq), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }

    private static IServiceCollection AddHangfireImplementation(this IServiceCollection services, string connectionString)
    {
        var hangfireConnectionString = $"{connectionString};Database=hangfire";

        services.AddHangfire(config => config
            .UseInMemoryStorage()
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings());

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
            options.Queues = ["default"];
        });

        return services;
    }

    public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = [new AllowAllConnectionsFilter()],
            DashboardTitle = "Jobs Dashboard"
        });

        return app;
    }

    private class AllowAllConnectionsFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}

public static class PipelineConfig
{
    public static readonly Action<IBusFactoryConfigurator, IRegistrationContext> ConfigureMassTransitPipeline = (cfg, context) =>
    {
        cfg.UseConsumeFilter(typeof(UnhandledExceptionFilter<>), context);
        cfg.UseConsumeFilter(typeof(PerformanceFilter<>), context);
        cfg.UseConsumeFilter(typeof(LoggingFilter<>), context);
        cfg.UseConsumeFilter(typeof(AuthorizationFilter<>), context);
        cfg.UseConsumeFilter(typeof(ValidationFilter<>), context);
        cfg.UseConsumeFilter(typeof(UnitOfWorkFilter<>), context);
    };
}
