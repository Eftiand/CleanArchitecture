namespace CleanArchitecture.Shared.Contracts.Messaging;

public interface ICommand<out TResponse> : ICommand;
public interface ICommand;
