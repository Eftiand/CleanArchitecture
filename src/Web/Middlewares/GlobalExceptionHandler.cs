using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception, "Exception occurred: {Message}", exception.Message);
        int statusCode = StatusCodes.Status401Unauthorized;
        if (exception.Data.Contains("StatusCode") && exception.Data["StatusCode"] is int code)
        {
            statusCode = code;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "Server error",
            Detail = exception.Message,
            Instance = exception.StackTrace,
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
