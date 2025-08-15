namespace AstraID.Application.Common;

public readonly struct Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    private Result(bool ok, T? value, string? code, string? message)
    {
        IsSuccess = ok;
        Value = value;
        ErrorCode = code;
        ErrorMessage = message;
    }
    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Failure(string code, string message) => new(false, default, code, message);
}

public readonly struct Result
{
    public bool IsSuccess { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    private Result(bool ok, string? code, string? message)
    {
        IsSuccess = ok;
        ErrorCode = code;
        ErrorMessage = message;
    }
    public static Result Success() => new(true, null, null);
    public static Result Failure(string code, string message) => new(false, code, message);
}
