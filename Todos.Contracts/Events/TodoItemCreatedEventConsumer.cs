using coaches.Modules.Shared.Contracts.Consumers;
using coaches.Modules.Shared.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Todos.Domain.Entities;

namespace coaches.Modules.Todos.Contracts.Events;

public class TodoItemCreatedEventConsumer(ILogger<TodoItemCreatedEventConsumer> logger)
    : BaseConsumer<EntityCreatedEvent<TodoItem>>
{
    protected override Task Consume(ConsumeContext<EntityCreatedEvent<TodoItem>> context)
    {
        logger.LogInformation("coaches Domain Event: {DomainEvent}", Message.GetType().Name);

        return Task.CompletedTask;
    }
}
