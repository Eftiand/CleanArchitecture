using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Modules.Shared.Contracts.Consumers;
using coaches.Modules.Shared.Contracts.Models;
using coaches.Modules.Todos.Contracts.Commands;
using coaches.Modules.Todos.Domain.Entities;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Exceptions;

namespace coaches.Modules.Todos.Application.Consumers;

public class UpdateTodoItemCommandConsumer(
    IApplicationDbContext dbContext
) : BaseConsumer<UpdateTodoItemCommand, Result<Guid>>
{
    protected override async Task<Result<Guid>> ConsumeWithResponse(ConsumeContext<UpdateTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = await dbContext
            .TodoItems
            .FirstOrDefaultAsync(x => x.Id == request.TodoId, context.CancellationToken);

        if (entity is null)
        {
            throw CommonExceptions.DomainExceptions.NotFound<TodoItem>();
        }

        entity.Title = request.Title;
        entity.Done = request.IsDone;

        return Result<Guid>.Success(entity.Id);
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
