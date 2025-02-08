using coaches.Modules.Shared.Application.Common.Models;
using coaches.Modules.Shared.Contracts.Events.@base;

namespace coaches.Modules.Todos.Contracts.Commands;

public record CreateTodoItemCommand(int ListId, string? Title = null) : BaseCommand<Result<Guid>>;
