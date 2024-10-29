using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace CleanArchitecture.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    : BaseConsumer<TodoItemCreatedEvent>
{
    public override Task Consume(ConsumeContext<TodoItemCreatedEvent> context)
    {
        logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", Message.GetType().Name);

        context.RespondAsync(new TodoCreatedResponse(context.Message.Item.Id));

        return Task.CompletedTask;
    }
}
