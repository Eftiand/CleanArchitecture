using System.Data.Common;
using coaches.Application.FunctionalTests.TestingFrameWork.Fixtures;
using coaches.Infrastructure;
using coaches.Infrastructure.Common.Extensions;
using coaches.Infrastructure.Data;
using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Web;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace coaches.Application.FunctionalTests.TestingFrameWork.App;

using static IntegrationBaseFunctionality;

public sealed class CustomWebApplicationFactory(DbConnection connection, Action<IRegistrationConfigurator> configureConsumers)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(_ => Mock.Of<IUser>(s => s.Id == GetUserId()));

            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.UseNpgsql(connection);
                });

            this.RemoveServicesForTests(services);
            services.AddMassTransitTestHarness(config =>
            {
                configureConsumers.Invoke(config);
                config.UsingInMemory((context, cfg) =>
                {
                    PipelineConfig.ConfigureMassTransitPipeline(cfg, context);
                    cfg.ConfigureEndpoints(context);
                });
            });
        });
    }

    private void RemoveServicesForTests(IServiceCollection services)
    {
        // Has to be spelled exactly like this to see the correct namespace.
        services.RemoveServicesFromNamespace("MassTransit");
    }
}
