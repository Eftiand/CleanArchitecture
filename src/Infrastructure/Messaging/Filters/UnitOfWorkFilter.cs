using coaches.Modules.Shared.Application.Common.Interfaces;
using coaches.Modules.Shared.Contracts.Common.Interfaces;
using MassTransit;

namespace coaches.Infrastructure.Messaging.Filters;

public class UnitOfWorkFilter<T>(IApplicationDbContext applicationDbContext)
    : IFilter<ConsumeContext<T>>
    where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        await next.Send(context);
        await applicationDbContext.SaveChangesAsync(context.CancellationToken);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("unit-of-work-filter");
    }
}
