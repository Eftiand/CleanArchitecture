using CleanArchitecture.Shared.Contracts.Messaging;
using CleanArchitecture.Shared.Contracts.Todos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.EventConsumers;

public class TodoItemCreatedEventConsumer(ILogger<TodoItemCreatedEventConsumer> logger)
    : BaseConsumer<TodoCreatedEvent>
{
    public override Task Consume(ConsumeContext<TodoCreatedEvent> context)
    {
        logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", Message.GetType().Name);

        return Task.CompletedTask;
    }
}
