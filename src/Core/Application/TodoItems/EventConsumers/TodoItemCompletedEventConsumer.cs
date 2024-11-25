using CleanArchitecture.Shared.Contracts.Messaging;
using CleanArchitecture.Shared.Contracts.Todos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.EventConsumers;

public class TodoItemCompletedEventConsumer(ILogger<TodoItemCompletedEventConsumer> logger)
    : BaseConsumer<TodoItemCompletedEvent, Guid>
{
    public override Task Consume(ConsumeContext<TodoItemCompletedEvent> context)
    {
        logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", Message.GetType().Name);

        return Task.CompletedTask;
    }
}
