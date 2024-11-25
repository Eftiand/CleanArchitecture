using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Shared.Contracts.Messaging;
using CleanArchitecture.Shared.Contracts.Todos;
using MassTransit;

namespace CleanArchitecture.Application.TodoItems.Consumers;

public class CreateTodoItemCommandConsumer(
    IApplicationDbContext dbContext)
    : BaseConsumer<CreateTodoItemCommand, TodoItem>
{
    public override async Task Consume(ConsumeContext<CreateTodoItemCommand> context)
    {
        var request = context.Message;
        var entity = new TodoItem { Title = request.Title, Done = false };

        entity.AddDomainEvent(new TodoCreatedEvent(entity.Id));

        await dbContext.TodoItems.AddAsync(entity, context.CancellationToken);

        await context.RespondAsync(new TodoItemCreatedResponse(entity.Id));
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
