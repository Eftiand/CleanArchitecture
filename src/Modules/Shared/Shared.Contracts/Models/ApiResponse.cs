namespace coaches.Modules.Shared.Contracts.Models;

public record class ApiResponse<TData>
{
    public required bool Success { get; init; }
    public required TData? Data { get; init; }
    public string? Message { get; init; }
    public required RequestMetadata RequestMetadata { get; init; }
}

public record class RequestMetadata
{
    public string TraceId { get; init; } = string.Empty;
    public string SpanId { get; init; } = string.Empty;
    public string? ParentSpanId { get; init; }
    public DateTime Timestamp { get; init; }
}

public record class ErrorDetails
{
    public required string Code { get; init; }
    public required string Message { get; init; }
}
