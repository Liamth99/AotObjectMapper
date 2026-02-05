namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// An attribute used to specify the mapping between a source property or field
/// and a destination property or field within the context of a class-level mapping.
/// </summary>
/// <typeparam name="TSource">The type of the source class being mapped from.</typeparam>
/// <typeparam name="TDestination">The type of the destination class being mapped to.</typeparam>
/// <param name="sourcePropertyName">The name of the property or field in the source class.</param>
/// <param name="destinationPropertyName">The name of the property or field in the destination class.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MapToAttribute<TSource, TDestination>(string sourcePropertyName, string destinationPropertyName) : Attribute
{
    /// Gets the name of the property or field in the source class that is mapped to a corresponding property or field in the destination class.
    public string SourcePropertyName      { get; } = sourcePropertyName;

    /// Gets the name of the property or field in the destination class that is mapped from a corresponding property or field in the source class.
    public string DestinationPropertyName { get; } = destinationPropertyName;
}