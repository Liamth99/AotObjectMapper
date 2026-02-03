namespace AotObjectMapper.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when the maximum allowable mapping depth is exceeded in a mapping operation.
/// </summary>
public class MaxMapDepthException : Exception
{
    public MaxMapDepthException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public MaxMapDepthException(string? message) : base(message)
    { }
}