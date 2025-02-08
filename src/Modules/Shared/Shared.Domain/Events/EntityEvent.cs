using coaches.Modules.Shared.Domain.BaseEntities.Base;

namespace coaches.Modules.Shared.Domain.Events;

public abstract record EntityEvent<T> : BaseIntegrationEvent<T>
{
    protected EntityEvent(T data, EntityEventType eventType)
    {
        Data = data;
        EventType = eventType;
    }

    public T Data { get; set; } = default!;
    public EntityEventType EventType { get; set; }
}

public record EntityCreatedEvent<T> : EntityEvent<T>
{
    public EntityCreatedEvent(T data) : base(data, EntityEventType.Created) { }
}

public record EntityUpdatedEvent<T> : EntityEvent<T>
{
    public EntityUpdatedEvent(T data) : base(data, EntityEventType.Updated) { }
}

public record EntityDeletedEvent<T> : EntityEvent<T>
{
    public EntityDeletedEvent(T data) : base(data, EntityEventType.Deleted) { }
}
