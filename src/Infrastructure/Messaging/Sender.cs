using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.Messaging;

namespace CleanArchitecture.Infrastructure.Messaging;

public class Sender(IServiceProvider serviceProvider)
    : ISender
{
    public async Task<TResponse> SendAsync<TCommand, TResponse>(TCommand message, CancellationToken cancellationToken = default)
        where TCommand : class
        where TResponse : class
    {
        var client = serviceProvider.GetRequiredService<IRequestClient<TCommand>>();
        var response = await client.GetResponse<TResponse>(message, cancellationToken);
        return response.Message;
    }
}
