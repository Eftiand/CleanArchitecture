using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Shared.Contracts.Messaging;

namespace CleanArchitecture.Domain.Common.BaseEntities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    private readonly List<ICommand> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<ICommand> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(ICommand domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(ICommand domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
