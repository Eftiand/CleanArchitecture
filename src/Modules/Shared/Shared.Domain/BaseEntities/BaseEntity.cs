using System.ComponentModel.DataAnnotations.Schema;

namespace coaches.Modules.Shared.Domain.BaseEntities;

public abstract class BaseEntity
{
    private readonly List<ICommand> _domainEvents = new();
    public Guid Id { get; set; }

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
