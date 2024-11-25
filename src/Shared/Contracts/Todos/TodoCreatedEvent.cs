using CleanArchitecture.Shared.Contracts.Messaging;

namespace CleanArchitecture.Shared.Contracts.Todos;

public record TodoCreatedEvent(Guid TodoId) : BaseEvent;
