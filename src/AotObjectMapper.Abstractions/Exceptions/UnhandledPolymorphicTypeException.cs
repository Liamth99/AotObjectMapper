namespace AotObjectMapper.Abstractions.Exceptions;

/// Represents an exception thrown when a polymorphic type cannot be properly handled or resolved.
public class UnhandledPolymorphicTypeException : Exception
{
    ///
    public UnhandledPolymorphicTypeException(string? message, Exception? innerException) : base(message, innerException)
    { }

    ///
    public UnhandledPolymorphicTypeException(string? message) : base(message)
    { }
}