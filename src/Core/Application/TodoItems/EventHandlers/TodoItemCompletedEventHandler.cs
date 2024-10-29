using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.EventHandlers;

public class TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    : BaseConsumer<TodoItemCompletedEvent, Guid>
{
    public override Task Consume(ConsumeContext<TodoItemCompletedEvent> context)
    {
        logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", Message.GetType().Name);

        return Task.CompletedTask;
    }
}
