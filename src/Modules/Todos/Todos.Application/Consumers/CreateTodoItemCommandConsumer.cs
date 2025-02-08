using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Modules.Shared.Contracts.Consumers;
using coaches.Modules.Shared.Contracts.Models;
using coaches.Modules.Todos.Contracts.Commands;
using coaches.Modules.Todos.Domain.Entities;
using FluentValidation;
using MassTransit;

namespace coaches.Modules.Todos.Application.Consumers;

public class CreateTodoItemCommandConsumer(
    IApplicationDbContext dbContext
) : BaseConsumer<CreateTodoItemCommand, Result<Guid>>
{
    protected override async Task<Result<Guid>> ConsumeWithResponse(ConsumeContext<CreateTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = new TodoItem
        {
            Title = request.Title,
            Done = false
        };

        await dbContext.TodoItems.AddAsync(entity, context.CancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
    }
}
