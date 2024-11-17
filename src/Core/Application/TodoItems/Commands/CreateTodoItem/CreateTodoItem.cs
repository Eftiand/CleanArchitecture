using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using MassTransit;

namespace CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand(int ListId, string? Title = null) : BaseCommand<TodoItemCreatedResponse>;
public record TodoItemCreatedResponse(Guid Id);

public class CreateTodoItemCommandHandler(
    IApplicationDbContext dbContext)
    : BaseConsumer<CreateTodoItemCommand, TodoItem>
{
    public override async Task Consume(ConsumeContext<CreateTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = new TodoItem
        {
            Title = request.Title,
            Done = false
        };

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        await dbContext.TodoItems.AddAsync(entity, context.CancellationToken);

        await context.RespondAsync(new TodoItemCreatedResponse(entity.Id));
    }
}
