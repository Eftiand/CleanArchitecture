namespace coaches.Modules.Shared.Domain.BaseEntities.Base;

public record BaseCommand<T> : BaseMessage, ICommand<T>;

public record BaseCommand : BaseCommand<bool>;