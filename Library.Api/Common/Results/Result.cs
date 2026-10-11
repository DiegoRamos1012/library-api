namespace Library.Api.Common.Results;

public sealed class Result<T>
{
    private readonly ResultError? _error;
    
    private Result(T? value, ResultError? error)
    {
        Value = value;
        _error = error;
    }

    private bool IsSuccess => _error is null;

    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("Cannot read the value from a failed Result.");

    public ResultError Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("Cannot read the error from a successful Result.");

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, null);
    }

    public static Result<T> Failure(ResultError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<T>(default, error);
    }

    public static Result<T> Failure(string code, string message)
    {
        return Failure(new ResultError(code, message));
    }
}