using System.Transactions;
using CleanArchitecture.Application.Common.Interfaces;
using MassTransit;

namespace CleanArchitecture.Infrastructure.Messaging.Filters;

public class UnitOfWorkFilter<T>(IApplicationDbContext applicationDbContext)
    : IFilter<ConsumeContext<T>>
    where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await next.Send(context);
        await applicationDbContext.SaveChangesAsync(context.CancellationToken);
        transactionScope.Complete();
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("unit-of-work-filter");
    }
}
