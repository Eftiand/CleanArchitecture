using System.Diagnostics;
using CleanArchitecture.Application.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Messaging.Filters;

public class PerformanceFilter<T>(
    ILogger<PerformanceFilter<T>> logger,
    IUser user,
    IIdentityService identityService)
    : IFilter<ConsumeContext<T>>
    where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var timer = Stopwatch.StartNew();

        try
        {
            await next.Send(context);
        }
        finally
        {
            timer.Stop();
            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var messageName = typeof(T).Name;
                var userId = user.Id ?? string.Empty;
                var userName = string.Empty;

                if (!string.IsNullOrEmpty(userId))
                {
                    userName = await identityService.GetUserNameAsync(userId);
                }

                logger.LogWarning("CleanArchitecture Long Running Message: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Message}",
                    messageName, elapsedMilliseconds, userId, userName, context.Message);
            }
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("performance-filter");
    }
}
