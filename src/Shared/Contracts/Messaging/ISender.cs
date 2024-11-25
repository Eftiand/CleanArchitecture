namespace CleanArchitecture.Shared.Contracts.Messaging;

public interface ISender
{
    Task<TResponse> SendAsync<TCommand, TResponse>(TCommand message, CancellationToken cancellationToken = default)
        where TCommand : class
        where TResponse : class;
}
