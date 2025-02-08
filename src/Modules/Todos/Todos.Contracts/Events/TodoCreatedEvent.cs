using coaches.Modules.Shared.Domain.BaseEntities;

namespace coaches.Modules.Todos.Contracts.Events;

public record TodoCreatedEvent(Guid TodoId) : BaseEvent;
