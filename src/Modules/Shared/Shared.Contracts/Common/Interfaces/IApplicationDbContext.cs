using coaches.Modules.Todos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace coaches.Modules.Shared.Contracts.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
