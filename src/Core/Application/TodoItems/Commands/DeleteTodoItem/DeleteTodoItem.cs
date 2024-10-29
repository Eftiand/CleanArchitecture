using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using MassTransit;

namespace CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(Guid Id) : BaseCommand<Guid>;

public record TodoItemDeletedResponse(Guid Id);

public class DeleteTodoItemCommandHandler(
    IApplicationDbContext dbContext)
    : BaseConsumer<DeleteTodoItemCommand, Guid>
{
    public override async Task Consume(ConsumeContext<DeleteTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = await dbContext.TodoItems
            .FindAsync(request.Id);

        Guard.Against.NotFound(request.Id, entity);

        dbContext.TodoItems.Remove(entity);

        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        await context.RespondAsync(new TodoItemDeletedResponse(entity.Id));
    }
}
