using CleanArchitecture.Shared.Contracts.Messaging;

namespace CleanArchitecture.Shared.Contracts.Todos;

public record DeleteTodoItemCommand(Guid TodoId) : BaseCommand<Guid>;
