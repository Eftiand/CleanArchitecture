using CleanArchitecture.Domain.Common.BaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Data.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SoftDeleteEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SoftDeleteEntities(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SoftDeleteEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var deletedEntries = context.ChangeTracker
            .Entries<ISoftDelete>()
            .Where(x => x.State == EntityState.Deleted);

        foreach (var entry in deletedEntries)
        {
            entry.State = EntityState.Modified;
            var entity = entry.Entity;
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }
}
