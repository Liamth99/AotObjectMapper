namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Specifies the mapping configuration between two types for use with a mapping generator.
/// </summary>
/// <typeparam name="TSource">The source type to map from.</typeparam>
/// <typeparam name="TDestination">The destination type to map to.</typeparam>
/// <param name="ignoredMembers">
/// An optional list of member names to ignore during the mapping process. These members will not be included in the mapping logic.
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MapAttribute<TSource, TDestination>(params string[] ignoredMembers) : Attribute
{
    /// Represents the collection of member names that should be ignored during the mapping process.
    public IEnumerable<string> IgnoredMembers { get; } = ignoredMembers;
}