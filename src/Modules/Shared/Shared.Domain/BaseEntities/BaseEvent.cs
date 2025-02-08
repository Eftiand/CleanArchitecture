using coaches.Modules.Shared.Domain.BaseEntities.Base;

namespace coaches.Modules.Shared.Domain.BaseEntities;

public abstract record BaseEvent : BaseMessage, IDomainEvent;
