using System.Data.Common;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IUser = CleanArchitecture.Application.Common.Interfaces.IUser;

namespace CleanArchitecture.Application.FunctionalTests.TestingFrameWork;

using static Testing;

public sealed class CustomWebApplicationFactory(DbConnection connection, Action<IBusRegistrationConfigurator>? configureConsumers = null)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(provider => Mock.Of<IUser>(s => s.Id == GetUserId()));

            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.UseNpgsql(connection);
                });
                services.AddMassTransitTestHarness(cfg =>
                {
                    configureConsumers?.Invoke(cfg);
                    cfg.UsingInMemory((context, cfg) =>
                    {
                        PipelineConfig.ConfigurePipeline(cfg, context);

                        // Configure endpoints explicitly
                        cfg.ConfigureEndpoints(context);
                    });
                });
        });
    }
}
