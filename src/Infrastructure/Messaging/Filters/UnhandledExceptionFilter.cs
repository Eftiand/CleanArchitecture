using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Messaging.Filters;

public class UnhandledExceptionFilter<TMessage>(ILogger<UnhandledExceptionFilter<TMessage>> logger)
    : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
    {
        try
        {
            await next.Send(context);
        }
        catch (Exception ex)
        {
            var messageType = typeof(TMessage).Name;
            logger.LogError(ex, "CleanArchitecture Consumer: Unhandled Exception for Message {Name} {@Message}", messageType, context.Message);
            throw;

        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("unhandled-exception-filter");
    }
}
