namespace AotObjectMapper.Abstractions.Exceptions;

public class EnumMissingFieldException : Exception
{
    public EnumMissingFieldException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public EnumMissingFieldException(string? message) : base(message)
    { }
}