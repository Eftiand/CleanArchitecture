using CleanArchitecture.Shared.Contracts.Messaging;

namespace CleanArchitecture.Infrastructure.Common.Extensions;

public static class SenderExtensions
{
    public static Task<TResponse> SendAsync<TResponse>(this ISender sender, object message, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        var messageType = message.GetType();
        var method = sender.GetType().GetMethod(nameof(ISender.SendAsync));
        var genericMethod = method?.MakeGenericMethod(messageType, typeof(TResponse));
        return (Task<TResponse>)genericMethod?.Invoke(sender, [message, cancellationToken])!;
    }
}
