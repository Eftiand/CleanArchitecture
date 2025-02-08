using System.Linq.Expressions;
using System.Reflection;
using coaches.Infrastructure.Identity;
using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Modules.Shared.Domain.BaseEntities;
using coaches.Modules.Todos.Domain.Entities;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace coaches.Infrastructure.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<OutboxState> OutboxState => Set<OutboxState>();
    public DbSet<InboxState> InboxStates => Set<InboxState>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply soft delete
        ApplySoftDeleteQueryFilter(builder);

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }

    private void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var filterExpression = CreateSoftDeleteFilterExpression(entityType.ClrType);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filterExpression);
            }
        }
    }

    private LambdaExpression CreateSoftDeleteFilterExpression(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var propertyAccess = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
        var notDeletedExpression = Expression.Equal(propertyAccess, Expression.Constant(false));
        return Expression.Lambda(notDeletedExpression, parameter);
    }
}
