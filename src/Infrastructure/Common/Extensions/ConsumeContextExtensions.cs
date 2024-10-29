using System.Text.Json;
using CleanArchitecture.Application.Common.Models;
using MassTransit;

namespace CleanArchitecture.Infrastructure.Common.Extensions;

public static class ConsumeContextExtensions
{
    public static Task NotifyFilterFault<T>(this ConsumeContext<T> context, string error, int httpStatusCode = 500) where T : class
    {
        var result = Result.Failure(error);
        var exception = new UnauthorizedAccessException(JsonSerializer.Serialize(result.Errors)) { Data = { ["StatusCode"] = httpStatusCode } };
        context.NotifyFaulted(TimeSpan.Zero, $"{typeof(T).Name}", exception);
        throw exception;
    }
}
