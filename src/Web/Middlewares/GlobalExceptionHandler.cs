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
        int statusCode = StatusCodes.Status500InternalServerError;
        if (exception is RequestFaultException requestFaultException)
        {
            var faultException = requestFaultException?.Fault?.Exceptions.SingleOrDefault();
            if (faultException != null)
            {
            }
        }

        logger.LogError(
            exception, "Exception occurred: {Message}", exception.Message);
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
