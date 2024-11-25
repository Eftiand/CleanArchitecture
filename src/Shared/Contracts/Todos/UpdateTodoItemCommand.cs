using CleanArchitecture.Shared.Contracts.Messaging;

namespace CleanArchitecture.Shared.Contracts.Todos;

public record UpdateTodoItemCommand(Guid TodoId, string Title, bool IsDone ) : BaseCommand;
