using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.FeatureName.Commands.CleanArchitectureUseCase;

public record CleanArchitectureUseCaseCommand : BaseCommand<object>
{
}

public class CleanArchitectureUseCaseCommandValidator : AbstractValidator<CleanArchitectureUseCaseCommand>
{
    public CleanArchitectureUseCaseCommandValidator()
    {
    }
}

public class CleanArchitectureUseCaseCommandHandler(
    IApplicationDbContext
    ) : BaseConsumer<CleanArchitectureUseCaseCommand, object>
{
    public override async Task Consume(ConsumerContext<CreateTodoItemCommand> context)
    {
        throw new NotImplementedException();
    }
}
