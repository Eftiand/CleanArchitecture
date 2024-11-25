using CleanArchitecture.Shared.Contracts.Messaging;

namespace CleanArchitecture.Shared.Contracts.Todos;

public record TodoItemCompletedEvent(Guid TodoId) : BaseEvent;
