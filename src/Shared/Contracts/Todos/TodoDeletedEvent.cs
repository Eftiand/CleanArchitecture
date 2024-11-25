using CleanArchitecture.Shared.Contracts.Messaging;

namespace CleanArchitecture.Shared.Contracts.Todos;

public record TodoDeletedEvent(Guid TodoId) : BaseEvent;
