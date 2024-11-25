using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Shared.Contracts.Messaging;
using CleanArchitecture.Shared.Contracts.Todos;
using Mapster;
using MassTransit;

namespace CleanArchitecture.Application.TodoItems.Consumers;

public class UpdateTodoItemCommandConsumer(
    IApplicationDbContext dbContext)
    : BaseConsumer<UpdateTodoItemCommand, TodoItem>
{
    public override async Task Consume(ConsumeContext<UpdateTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = await dbContext.TodoItems.
            FirstOrDefaultAsync(x => x.Id == request.TodoId, context.CancellationToken);

        if (entity is null)
        {
            throw CommonExceptions.DomainExceptions.NotFound<TodoItem>();
        }

        entity.Adapt(request);
    }
}

public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}
