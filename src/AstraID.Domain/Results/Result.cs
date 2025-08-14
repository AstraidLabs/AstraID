using AstraID.Domain.Errors;

namespace AstraID.Domain.Results;

/// <summary>Represents the outcome of an operation.</summary>
public readonly struct Result
{
    /// <summary>Indicates success.</summary>
    public bool IsSuccess { get; }

    /// <summary>Error when <see cref="IsSuccess"/> is false.</summary>
    public DomainError? Error { get; }

    private Result(bool ok, DomainError? error)
    {
        IsSuccess = ok;
        Error = error;
    }

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(true, null);

    /// <summary>Creates a failure result.</summary>
    public static Result Failure(DomainError error) => new(false, error);
}

/// <summary>Represents the outcome of an operation returning a value.</summary>
public readonly struct Result<T>
{
    /// <summary>Indicates success.</summary>
    public bool IsSuccess { get; }

    /// <summary>Returned value.</summary>
    public T? Value { get; }

    /// <summary>Error when <see cref="IsSuccess"/> is false.</summary>
    public DomainError? Error { get; }

    private Result(bool ok, T? value, DomainError? error)
    {
        IsSuccess = ok;
        Value = value;
        Error = error;
    }

    /// <summary>Creates a successful result with a value.</summary>
    public static Result<T> Success(T value) => new(true, value, null);

    /// <summary>Creates a failure result.</summary>
    public static Result<T> Failure(DomainError error) => new(false, default, error);
}
