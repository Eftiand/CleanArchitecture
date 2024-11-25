using CleanArchitecture.Application.FunctionalTests.TestingFrameWork;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Shared.Contracts.Todos;
using MassTransit;

namespace CleanArchitecture.Application.FunctionalTests.TodoItems.Commands;

public class CreateTodoItemTests : BaseTestFixture
{
    protected override void ConfigureConsumers(IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<CreateTodoItemCommandConsumer>();
    }

    [Test]
    public async Task ShouldConsumeCreateTodoItemCommand()
    {
        var command = new CreateTodoItemCommand(11111, "Hello");
        await SendAsync<CreateTodoItemCommand, TodoItemCreatedResponse>(command);

        Assert.That(await ConsumedAsync(command), Is.True);
    }
}
