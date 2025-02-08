namespace coaches.Modules.Shared.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    internal Result() { }

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; } = [];

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }

    public static Result Failure(string error)
    {
        return Failure([error]);
    }
}

public class Result<T> : Result
{
    private Result(bool succeeded, T data, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Data = data;
    }

    // Don't use this. Just for serialization
    public Result() { }

    public T Data { get; init; } = default!;

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, Array.Empty<string>());
    }

    public static new Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T>(false, default!, errors);
    }

    public static implicit operator Result<T>(T data)
    {
        return Success(data);
    }
}
