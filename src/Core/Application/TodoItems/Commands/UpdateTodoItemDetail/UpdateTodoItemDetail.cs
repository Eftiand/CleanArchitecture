using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using Mapster;
using MassTransit;
using static CleanArchitecture.Domain.Exceptions.CommonExceptions;

namespace CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail;

public record UpdateTodoItemDetailCommand(Guid Id, PriorityLevel PriorityLevel, string? Note) : BaseCommand<TodoItem>;

public record UpdateTodoItemResponse(Guid Id);

public class UpdateTodoItemDetailCommandHandler(
    IApplicationDbContext dbContext)
    : BaseConsumer<UpdateTodoItemDetailCommand, TodoItem>
{
    public override async Task Consume(ConsumeContext<UpdateTodoItemDetailCommand> context)
    {
        var request = context.Message;
        var entity = await dbContext.TodoItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, context.CancellationToken);

        if (entity is null)
        {
            throw DomainExceptions.NotFound<TodoItem>();
        }

        entity.Adapt(request);

        await context.RespondAsync(new UpdateTodoItemResponse(entity.Id));
    }
}
