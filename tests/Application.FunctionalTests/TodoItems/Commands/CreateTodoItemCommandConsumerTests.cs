using coaches.Application.FunctionalTests.TestingFrameWork.Fixtures;
using coaches.Modules.Shared.Application.Common.Models;
using coaches.Modules.Shared.Contracts.Events;
using coaches.Modules.Todos.Application.Consumers;
using coaches.Modules.Todos.Contracts.Commands;
using coaches.Modules.Todos.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Todos.Domain.Entities;

namespace coaches.Application.FunctionalTests.TodoItems.Commands;

public class CreateTodoItemTests : IntegrationTest
{
    public override void ConfigureConsumers(IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<CreateTodoItemCommandConsumer>();
        configurator.AddConsumer<TodoItemCreatedEventConsumer>();
    }

    [Test]
    public async Task ShouldConsumeCreateTodoItemCommand()
    {
        var command = new CreateTodoItemCommand(11111, "Hello");

        var result = await SendAsync<CreateTodoItemCommand, Result<Guid>>(command);
        var todoResponse = result.Data;

        await Task.WhenAll(
            ConsumedAsync<CreateTodoItemCommand>(assert: false),
            ConsumedAsync<EntityCreatedEvent<TodoItem>>(assert: false)
        );

        var todo = await this.Session
            .TodoItems
            .SingleOrDefaultAsync(x => x.Id == todoResponse);

        Assert.That(todo, Is.Not.Null);
        Assert.That(todo.Title, Is.EquivalentTo("Hello"));

        await ConsumedAsync<EntityCreatedEvent<TodoItem>>();
    }
}
