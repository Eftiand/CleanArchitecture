using coaches.Modules.Shared.Contracts.Models;
using coaches.Modules.Shared.Domain.BaseEntities.Base;

namespace coaches.Modules.Todos.Contracts.Commands;

public record CreateTodoItemCommand(int ListId, string? Title = null) : BaseCommand<Result<Guid>>;
