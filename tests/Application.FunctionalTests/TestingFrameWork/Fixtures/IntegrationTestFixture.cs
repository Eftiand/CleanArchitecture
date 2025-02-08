using coaches.Application.FunctionalTests.TestingFrameWork.App;
using coaches.Application.FunctionalTests.TestingFrameWork.Database;
using coaches.Infrastructure.Identity;
using coaches.Modules.Shared.Contracts.Common.Interfaces;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace coaches.Application.FunctionalTests.TestingFrameWork.Fixtures;

[SetUpFixture]
public abstract class IntegrationTestFixture
{
    // Static resources shared across all tests
    protected static ITestDatabase _database = null!;
    protected static CustomWebApplicationFactory _factory = null!;
    protected static IServiceScopeFactory _scopeFactory = null!;
    protected static ITestHarness _harness = null!;
    protected static string? _userId;

    // Instance-level resources per test
    protected IServiceScope _scope = null!;
    protected IApplicationDbContext DbContext => _scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();
        _factory = new CustomWebApplicationFactory(_database.GetConnection(), ConfigureConsumers);
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _harness = _factory.Services.GetRequiredService<ITestHarness>();
        await _harness.Start();
    }

    [SetUp]
    public virtual void SetUp()
    {
        _scope = _scopeFactory.CreateScope();
    }

    public virtual void ConfigureConsumers(IRegistrationConfigurator configurator)
    {
    }

    [TearDown]
    public async Task ResetState()
    {
        try
        {
            _scope.Dispose();
            await _database.ResetAsync();
        }
        catch (Exception)
        {
            // ignored
        }

        _userId = null;
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await _harness.Stop();
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
