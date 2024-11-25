namespace CleanArchitecture.Shared.Contracts.Messaging;

public record BaseCommand<T> : ICommand<T>;
public record BaseCommand : ICommand;
