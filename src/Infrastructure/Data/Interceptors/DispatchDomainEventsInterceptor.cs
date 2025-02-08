using coaches.Modules.Shared.Application.Common.Interfaces;
using coaches.Modules.Shared.Domain.BaseEntities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace coaches.Infrastructure.Data.Interceptors;

public class DispatchDomainEventsInterceptor(ISender sender) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return SavingChangesAsync(eventData, result).GetAwaiter().GetResult();
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null) return;
        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();
        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();
        entities.ToList().ForEach(e => e.ClearDomainEvents());
        foreach (var domainEvent in domainEvents)
            await sender.SendAsync(domainEvent);
    }
}
