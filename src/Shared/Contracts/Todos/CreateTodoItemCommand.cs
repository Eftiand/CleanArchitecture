using CleanArchitecture.Shared.Contracts.Messaging;

namespace CleanArchitecture.Shared.Contracts.Todos;

public record CreateTodoItemCommand(int ListId, string? Title = null) : BaseCommand<TodoItemCreatedResponse>;
