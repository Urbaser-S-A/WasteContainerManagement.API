namespace WCM.API.ApiService.Domain.Shared;

/// <summary>
/// Represents an error with a code and message.
/// </summary>
public sealed class Error : IEquatable<Error>
{
    /// <summary>
    /// Represents no error.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, 0);

    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the HTTP status code associated with this error.
    /// </summary>
    public int StatusCode { get; }

    public Error(string code, string message, int statusCode = StatusCodes.Status500InternalServerError)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        StatusCode = statusCode;
    }

    public bool Equals(Error? other) =>
        other is not null && Code == other.Code;

    public override bool Equals(object? obj) =>
        obj is Error other && Equals(other);

    public override int GetHashCode() =>
        Code.GetHashCode(StringComparison.Ordinal);

    public override string ToString() =>
        $"Error: {Code} - {Message}";
}
