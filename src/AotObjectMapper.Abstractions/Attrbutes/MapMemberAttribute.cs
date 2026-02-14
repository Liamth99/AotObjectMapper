namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// An attribute used to define a mapping between a source member and a destination member in classes
/// designed for object mapping. This attribute is applied at the class level and supports specifying
/// a pair of member names to establish the mapping.
/// </summary>
/// <typeparam name="TSource">The type of the source object.</typeparam>
/// <typeparam name="TDestination">The type of the destination object.</typeparam>
/// <param name="sourceMemberName">The name of the member in the source type to be mapped.</param>
/// <param name="destinationMemberName">The name of the member in the destination type to be mapped.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MapMemberAttribute<TSource, TDestination>(string sourceMemberName, string destinationMemberName) : Attribute
{
    /// The name of the member in the source object type that is being mapped to a corresponding member in the destination object type.
    public string SourceMemberName { get; } = sourceMemberName;

    /// The name of the member in the destination object type that is mapped to a corresponding member in the source object type.
    public string DestinationMemberName { get; } = destinationMemberName;
}