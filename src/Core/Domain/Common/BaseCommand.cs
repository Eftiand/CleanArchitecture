using CleanArchitecture.Domain.Messaging;

namespace CleanArchitecture.Domain.Common;

public record BaseCommand<T> : ICommand<T>;
public record BaseCommand : ICommand;
