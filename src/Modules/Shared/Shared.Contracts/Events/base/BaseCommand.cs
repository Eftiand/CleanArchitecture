using coaches.Modules.Shared.Domain.BaseEntities;

namespace coaches.Modules.Shared.Contracts.Events.@base;

public record BaseCommand<T> : ICommand<T>;

public record BaseCommand : BaseCommand<bool>;
