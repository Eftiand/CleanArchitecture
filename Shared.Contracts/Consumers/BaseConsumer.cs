using coaches.Modules.Shared.Domain.BaseEntities;
using MassTransit;

namespace coaches.Modules.Shared.Contracts.Consumers;

public abstract class BaseConsumer<TCommand> : IConsumer<TCommand>
    where TCommand : class
{
    private readonly List<object> _messagesToPublish = [];

    protected TCommand Message { get; private set; } = default!;

    async Task IConsumer<TCommand>.Consume(ConsumeContext<TCommand> context)
    {
        Message = context.Message;
        await Consume(context);

        foreach (object? message in _messagesToPublish)
        {
            await context.Publish(message);
        }
    }

    protected Task Publish(ICommand message)
    {
        _messagesToPublish.Add(message);
        return Task.CompletedTask;
    }

    protected abstract Task Consume(ConsumeContext<TCommand> context);
}

public abstract class BaseConsumer<TCommand, TResponse> : BaseConsumer<TCommand>
    where TCommand : class
{
    protected sealed override async Task Consume(ConsumeContext<TCommand> context)
    {
        TResponse? response = await ConsumeWithResponse(context);
        if (response is not null)
        {
            await context.RespondAsync(response);
        }
    }

    protected abstract Task<TResponse> ConsumeWithResponse(ConsumeContext<TCommand> context);
}
