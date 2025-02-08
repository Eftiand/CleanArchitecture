using coaches.Modules.Shared.Contracts.Events.@base;

namespace coaches.Modules.Todos.Contracts.Commands;

public record UpdateTodoItemCommand(Guid TodoId, string Title, bool IsDone) : BaseCommand;
