using MassTransit;

namespace CleanArchitecture.Domain.Common;

public abstract class BaseConsumer<TCommand, TResponse> : BaseConsumer<TCommand>
    where TCommand : class;

public abstract class BaseConsumer<TCommand>
    : IConsumer<TCommand> where TCommand : class
{
    protected TCommand Message { get; set; } = default!;
    public abstract Task Consume(ConsumeContext<TCommand> context);
}
