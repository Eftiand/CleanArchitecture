namespace CleanArchitecture.Domain.Messaging;

public interface ICommand<out TResponse> : ICommand;
public interface ICommand;
