namespace AstraID.Domain.Errors;

/// <summary>Represents a domain error with a code and message.</summary>
public sealed class DomainError
{
    /// <summary>Error code.</summary>
    public string Code { get; }

    /// <summary>Error message.</summary>
    public string Message { get; }

    private DomainError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>Creates a new <see cref="DomainError"/>.</summary>
    public static DomainError From(string code, string message) => new(code, message);
}
