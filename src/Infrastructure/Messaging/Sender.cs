using coaches.Modules.Shared.Contracts.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace coaches.Infrastructure.Messaging;

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

    public async Task SendAsync(object message, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();
        await publishEndpoint.Publish(message, cancellationToken);

        if (saveChanges)
        {
            var dbContext = serviceProvider.GetRequiredService<IApplicationDbContext>();
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
