namespace coaches.Modules.Shared.Application.Common.Interfaces;

public interface ISender
{
    Task<TResponse> SendAsync<TCommand, TResponse>(TCommand message, CancellationToken cancellationToken = default)
        where TCommand : class
        where TResponse : class;

    Task SendAsync(object message, bool saveChanges = true, CancellationToken cancellationToken = default);
}
