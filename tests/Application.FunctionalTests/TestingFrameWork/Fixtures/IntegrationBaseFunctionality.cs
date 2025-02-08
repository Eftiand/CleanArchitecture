using coaches.Infrastructure.Identity;
using coaches.Modules.Shared.Application.Common.Constants;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace coaches.Application.FunctionalTests.TestingFrameWork.Fixtures;

public abstract class IntegrationBaseFunctionality : IntegrationTestFixture
{
    protected async Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command)
        where TCommand : class
        where TResponse : class
    {
        var requestClient = _harness.Bus.CreateRequestClient<TCommand>();
        var response = await requestClient.GetResponse<TResponse>(command);
        await ResetScope();
        return response.Message;
    }

    protected async Task SendAsync<TCommand>(TCommand request)
        where TCommand : class
    {
        await _harness.Bus.Publish(request!);

        await ConsumedAsync(request);
        await ResetScope();
    }

    protected async Task<bool> ConsumedAsync<T>(T? message = null, bool assert = true) where T : class
    {
        var consumedTask = _harness.Consumed.Any<T>();

        var result = await consumedTask;
        if (assert)
        {
            Assert.That(result, Is.True);

        }

        return result;
    }

    protected async Task<bool> PublishedAsync<T>(T? message = null, bool assert = true) where T : class
    {
        var publishedTask = _harness.Published.Any<T>();

        var result = await publishedTask;
        if (assert)
        {
            Assert.That(result, Is.True);
        }

        return result;
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
        return await RunAsUserAsync("administrator@local", "Administrator1234!", new[]
        {
            Roles.Administrator
        });
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName
        };

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

    protected Task ResetScope()
    {
        _scope.Dispose();
        _scope = _scopeFactory.CreateScope();

        return Task.CompletedTask;
    }
}
