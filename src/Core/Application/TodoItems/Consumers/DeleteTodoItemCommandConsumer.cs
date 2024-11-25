using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Shared.Contracts.Messaging;
using CleanArchitecture.Shared.Contracts.Todos;
using MassTransit;

namespace CleanArchitecture.Application.TodoItems.Consumers;

public record TodoItemDeletedResponse(Guid Id);

public class DeleteTodoItemCommandHandler(
    IApplicationDbContext dbContext)
    : BaseConsumer<DeleteTodoItemCommand, Guid>
{
    public override async Task Consume(ConsumeContext<DeleteTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = await dbContext.TodoItems
            .FindAsync(request.TodoId);

        Guard.Against.NotFound(request.TodoId, entity);

        dbContext.TodoItems.Remove(entity);

        entity.AddDomainEvent(new TodoDeletedEvent(entity.Id));

        await context.RespondAsync(new TodoItemDeletedResponse(entity.Id));
    }
}
