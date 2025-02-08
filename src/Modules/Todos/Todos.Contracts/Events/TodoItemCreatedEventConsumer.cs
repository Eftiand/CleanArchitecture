using coaches.Modules.Shared.Contracts.Consumers;
using coaches.Modules.Shared.Domain.Events;
using coaches.Modules.Todos.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

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
