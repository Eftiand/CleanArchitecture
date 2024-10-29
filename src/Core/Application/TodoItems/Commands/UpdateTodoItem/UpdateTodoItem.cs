using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using Mapster;
using MassTransit;

namespace CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand : BaseCommand<TodoItem>
{
    public Guid Id { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }
}

public class UpdateTodoItemCommandHandler(
    IApplicationDbContext dbContext)
    : BaseConsumer<UpdateTodoItemCommand, TodoItem>
{
    public override async Task Consume(ConsumeContext<UpdateTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = await dbContext.TodoItems.
            FirstOrDefaultAsync(x => x.Id == request.Id, context.CancellationToken);

        if (entity is null)
        {
            throw CommonExceptions.DomainExceptions.NotFound<TodoItem>();
        }

        entity.Adapt(request);
    }
}
