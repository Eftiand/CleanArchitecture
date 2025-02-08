using coaches.Modules.Shared.Contracts.Events.@base;

namespace coaches.Modules.Todos.Contracts.Commands;

public record DeleteTodoItemCommand(Guid TodoId) : BaseCommand<Guid>;
