namespace coaches.Modules.Shared.Domain.BaseEntities;

public interface ICommand<out TResponse> : IDomainEvent, ICommand;

public interface ICommand;
