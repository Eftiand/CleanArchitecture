using coaches.Modules.Shared.Domain.BaseEntities.Base;

namespace coaches.Modules.Todos.Contracts.Commands;

public record DeleteTodoItemCommand(Guid TodoId) : BaseCommand<Guid>;
