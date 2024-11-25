using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Identity;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ICommand = CleanArchitecture.Shared.Contracts.Messaging.ICommand;

namespace CleanArchitecture.Application.FunctionalTests.TestingFrameWork;

[SetUpFixture]
public abstract class Testing
{
    private static ITestDatabase _database = null!;
    private static CustomWebApplicationFactory _factory = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static string? _userId;
    private ITestHarness _harness { get; set; } = null!;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();

        _factory = new CustomWebApplicationFactory(_database.GetConnection(), ConfigureConsumers);

        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

        _harness = _factory.Services.GetRequiredService<ITestHarness>();
        await _harness.Start();
    }

    /// <summary>
    /// Consumers to run during the test.
    /// </summary>
    /// <param name="configurator"></param>
    protected virtual void ConfigureConsumers(IBusRegistrationConfigurator configurator)
    {
    }

    protected async Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command)
        where TCommand : class
        where TResponse : class
    {
        try
        {
            var requestClient = _harness.Bus.CreateRequestClient<TCommand>(TimeSpan.FromSeconds(5));
            var response = await requestClient.GetResponse<TResponse>(command);
            return response.Message;
        }
        catch (RequestTimeoutException ex)
        {
            var consumed = await _harness.Consumed.Any<TCommand>();
            throw new Exception($"Request timed out. Message consumed: {consumed}", ex);
        }
    }

    protected async Task SendAsync(ICommand request)
    {
        await _harness.Bus.Publish(request);
    }

    protected async Task<bool> ConsumedAsync<T>(T message) where T : class
    {
        return await _harness.Consumed.Any<T>(x => x.Context.Message.Equals(message));
    }

    public static string? GetUserId()
    {
        return _userId;
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>());
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { Roles.Administrator });
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = userName, Email = userName };

        var result = await userManager.CreateAsync(user, password);

        if (roles.Length != 0)
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            await userManager.AddToRolesAsync(user, roles);
        }

        if (result.Succeeded)
        {
            _userId = user.Id;

            return _userId;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    protected static async Task ResetState()
    {
        try
        {
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
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
