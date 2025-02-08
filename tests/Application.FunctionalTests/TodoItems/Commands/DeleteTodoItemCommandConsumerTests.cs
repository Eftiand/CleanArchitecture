using coaches.Application.FunctionalTests.TestingFrameWork.Fixtures;
using coaches.Modules.Shared.Domain.Events;
using coaches.Modules.Todos.Application.Consumers;
using coaches.Modules.Todos.Contracts.Commands;
using coaches.Modules.Todos.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace coaches.Application.FunctionalTests.TodoItems.Commands;

public class DeleteTodoItemCommandConsumerTests : IntegrationTest
{
    public override void ConfigureConsumers(IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<DeleteTodoItemCommandConsumer>();
    }

    [Test]
    public async Task ShouldConsumeDeleteTodoItemCommand()
    {
        await this.Session.TodoItems.AddAsync(TodoItem.Create("Testing", "Testing"));
        await this.Session.SaveChangesAsync();

        var todoCreated = await this.Session.TodoItems.ToListAsync();

        var command = new DeleteTodoItemCommand(
            TodoId: todoCreated.First().Id);

        await SendAsync(command);

        await ConsumedAsync(command);

        var todo = await this.Session
            .TodoItems
            .FirstOrDefaultAsync(x => x.Id == todoCreated.First().Id);

        Assert.That(todo, Is.Null);

        await PublishedAsync<EntityDeletedEvent<TodoItem>>();
    }
}
