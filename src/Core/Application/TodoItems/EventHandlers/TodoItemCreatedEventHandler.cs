using CleanArchitecture.Shared.Contracts.Messaging;
using CleanArchitecture.Shared.Contracts.Todos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    : BaseConsumer<TodoCreatedEvent>
{
    public override Task Consume(ConsumeContext<TodoCreatedEvent> context)
    {
        logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", Message.GetType().Name);

        context.RespondAsync(new TodoItemCreatedResponse(context.Message.TodoId));

        return Task.CompletedTask;
    }
}
