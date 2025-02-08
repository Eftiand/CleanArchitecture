using coaches.Application.FunctionalTests.TestingFrameWork.Fixtures;
using coaches.Modules.Shared.Application.Common.Models;
using coaches.Modules.Shared.Contracts.Events;
using coaches.Modules.Todos.Application.Consumers;
using coaches.Modules.Todos.Contracts.Commands;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Todos.Domain.Entities;

namespace coaches.Application.FunctionalTests.TodoItems.Commands;

public class UpdateTodoItemCommandConsumerTests : IntegrationTest
{
    public override void ConfigureConsumers(IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<UpdateTodoItemCommandConsumer>();
    }

    [Test]
    public async Task ShouldConsumeUpdateTodoItemCommand()
    {
        await this.Session.TodoItems.AddAsync(TodoItem.Create("Testing", "Testing"));
        await this.Session.SaveChangesAsync();

        var todoCreated = await this.Session.TodoItems.FirstAsync();

        var command = new UpdateTodoItemCommand(
            TodoId: todoCreated.Id,
            Title: "Updated",
            IsDone: true);

        var result = await SendAsync<UpdateTodoItemCommand, Result<Guid>>(command);

        await Task.WhenAll(
            ConsumedAsync<UpdateTodoItemCommand>(assert: false),
            ConsumedAsync<EntityCreatedEvent<TodoItem>>(assert: false)
        );

        var todo = await this.Session
            .TodoItems
            .FirstOrDefaultAsync(x => x.Id == result.Data);

        Assert.That(todo, Is.Not.Null);
        Assert.That(todo.Title, Is.EqualTo("Updated"));

        await PublishedAsync<EntityUpdatedEvent<TodoItem>>();
    }
}
