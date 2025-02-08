using Microsoft.EntityFrameworkCore;
using Todos.Domain.Entities;

namespace coaches.Modules.Shared.Contracts.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
