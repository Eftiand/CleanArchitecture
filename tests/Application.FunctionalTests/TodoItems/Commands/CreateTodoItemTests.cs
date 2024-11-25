using CleanArchitecture.Application.FunctionalTests.TestingFrameWork;
using CleanArchitecture.Application.TodoItems.Consumers;
using CleanArchitecture.Shared.Contracts.Todos;
using MassTransit;
using Microsoft.EntityFrameworkCore;

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
        var result = await SendAsync<CreateTodoItemCommand, TodoItemCreatedResponse>(command);

        Assert.That(await ConsumedAsync(command), Is.True);

        var todo = await this.Session.TodoItems
            .SingleOrDefaultAsync(x => x.Id == result.TodoId);

        todo.Should().NotBeNull();
        todo!.Title.Should().BeEquivalentTo("Hello");
    }
}
