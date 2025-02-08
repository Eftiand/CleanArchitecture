using System.Diagnostics;
using coaches.Modules.Shared.Application.Common.Models;

namespace coaches.Web.Common;

public static class WebExtensions
{
    private static readonly ActivitySource ActivitySource = new("web-api");

    public static async Task<IResult> ToApiResponse<T>(this Task<T> task)
    {
        var data = await task;
        Result<T> result = data;
        return result.ToApiResponse();
    }

    public static IResult ToApiResponse<T>(this Result<T> result, string? message = null)
    {
        var activity = Activity.Current;

        if (result.Succeeded)
        {
            using var scope = ActivitySource.StartActivity("process-success-response");
            scope?.SetTag("result_type", typeof(T).Name);

            var response = new ApiResponse<T> { Success = true, Message = message, Data = result.Data, RequestMetadata = new RequestMetadata { TraceId = activity?.TraceId.ToString() ?? string.Empty, SpanId = activity?.SpanId.ToString() ?? string.Empty, ParentSpanId = activity?.ParentSpanId.ToString(), Timestamp = DateTime.UtcNow } };

            return Results.Ok(response);
        }

        using var errorScope = ActivitySource.StartActivity("ProcessErrorResponse");
        errorScope?.SetTag("error_count", result.Errors.Length);

        var errorResponse = new ApiResponse<ErrorDetails> { Success = false, Data = new ErrorDetails { Code = "ValidationError", Message = result.Errors.FirstOrDefault() ?? "An error occurred" }, RequestMetadata = new RequestMetadata { TraceId = activity?.TraceId.ToString() ?? string.Empty, SpanId = activity?.SpanId.ToString() ?? string.Empty, ParentSpanId = activity?.ParentSpanId.ToString(), Timestamp = DateTime.UtcNow } };

        return Results.BadRequest(errorResponse);
    }
}
