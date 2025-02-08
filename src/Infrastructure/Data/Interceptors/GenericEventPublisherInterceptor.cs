using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Modules.Shared.Domain.Events;
using coaches.Modules.Todos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace coaches.Infrastructure.Data.Interceptors;

/// <summary>
/// This publishes generic events for all entities.
/// </summary>
public class GenericEventPublisherInterceptor(ISender sender) : SaveChangesInterceptor
{
    private readonly List<Type> _entityTypesToPublish =
    [
        typeof(TodoItem)
    ];

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null) return result;

        var changes = dbContext.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => _entityTypesToPublish.Contains(e.Entity.GetType()))
            .ToList();

        foreach (var entry in changes)
        {
            var entity = entry.Entity;
            var entityType = entity.GetType();
            var eventToPublish = entry.State switch
            {
                EntityState.Added => CreateGenericEvent(typeof(EntityCreatedEvent<>), entityType, entity),
                EntityState.Modified => CreateGenericEvent(typeof(EntityUpdatedEvent<>), entityType, entity),
                EntityState.Deleted => CreateGenericEvent(typeof(EntityDeletedEvent<>), entityType, entity),
                _ => throw new ArgumentException($"Unexpected entity state: {entry.State}")
            };

            await sender.SendAsync(
                message: eventToPublish,
                saveChanges: false,
                cancellationToken: cancellationToken);
        }

        return result;
    }

    private static object CreateGenericEvent(Type eventType, Type entityType, object entity)
    {
        var constructedEventType = eventType.MakeGenericType(entityType);
        return Activator.CreateInstance(constructedEventType, entity)!;
    }
}
