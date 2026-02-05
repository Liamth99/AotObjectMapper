namespace AotObjectMapper.Abstractions.Exceptions;

/// <summary>
/// Represents an exception thrown when a required field in an enumeration is missing when attempting to map enums by feild name.
/// </summary>
public class EnumMissingFieldException : Exception
{
    ///
    public EnumMissingFieldException(string? message, Exception? innerException) : base(message, innerException)
    { }

    ///
    public EnumMissingFieldException(string? message) : base(message)
    { }
}