namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Specifies the mapping configuration between two types for use with a mapping generator.
/// </summary>
/// <typeparam name="TSource">The source type to map from.</typeparam>
/// <typeparam name="TDestination">The destination type to map to.</typeparam>
/// <param name="methodName">
/// The name of the method to generate for the mapping. If not specified, the default value is <see cref="MapAttribute&lt;TSource, TDestination&gt;.DefaultMapMethodName">DefaultMapMethodName</see>.
/// </param>
/// <param name="ignoredMembers">
/// An optional list of member names to ignore during the mapping process. These members will not be included in the mapping logic.
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MapAttribute<TSource, TDestination>(string methodName = MapAttribute<TSource, TDestination>.DefaultMapMethodName, params string[] ignoredMembers) : Attribute
{
    /// Represents the default method name used for generating mapping logic
    /// when no custom method name is specified in the mapping configuration.
    public const string DefaultMapMethodName = "Map";

    /// Gets the name of the method to be generated for the mapping logic.
    public string MethodName     { get; } = methodName;

    /// Represents the collection of member names that should be ignored during the mapping process.
    public IEnumerable<string> IgnoredMembers { get; } = ignoredMembers;
}