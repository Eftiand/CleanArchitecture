using coaches.Modules.Shared.Contracts.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace coaches.Modules.Todos.Contracts.Events;

public class TodoItemCompletedEventConsumer(ILogger<TodoItemCompletedEventConsumer> logger)
    : BaseConsumer<TodoItemCompletedEvent>
{
    protected override Task Consume(ConsumeContext<TodoItemCompletedEvent> context)
    {
        logger.LogInformation("coaches Domain Event: {DomainEvent}", Message.GetType().Name);

        return Task.CompletedTask;
    }
}
