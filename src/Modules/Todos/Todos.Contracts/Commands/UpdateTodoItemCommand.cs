using coaches.Modules.Shared.Domain.BaseEntities.Base;

namespace coaches.Modules.Todos.Contracts.Commands;

public record UpdateTodoItemCommand(Guid TodoId, string Title, bool IsDone) : BaseCommand;
