using Ardalis.GuardClauses;
using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Modules.Shared.Contracts.Consumers;
using coaches.Modules.Shared.Contracts.Models;
using coaches.Modules.Todos.Contracts.Commands;
using MassTransit;

namespace coaches.Modules.Todos.Application.Consumers;

public class DeleteTodoItemCommandConsumer(
    IApplicationDbContext dbContext)
    : BaseConsumer<DeleteTodoItemCommand, Result<Guid>>
{
    protected override async Task<Result<Guid>> ConsumeWithResponse(ConsumeContext<DeleteTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = await dbContext.TodoItems
            .FindAsync(request.TodoId);

        Guard.Against.NotFound(request.TodoId, entity);

        dbContext.TodoItems.Remove(entity);

        await dbContext.SaveChangesAsync();

        return Result<Guid>.Success(entity.Id);
    }
}
