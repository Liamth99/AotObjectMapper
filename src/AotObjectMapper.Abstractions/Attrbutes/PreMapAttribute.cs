namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Defines an attribute to mark a method as a pre-mapping step in the object mapping process.
/// This attribute can be used to execute custom logic or preprocessing before the source
/// object is mapped to the destination object.
/// </summary>
/// <typeparam name="TSource">The type of the source object involved in the mapping.</typeparam>
/// <typeparam name="TDestination">The type of the destination object involved in the mapping.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class PreMapAttribute<TSource, TDestination>(int priority = int.MaxValue) : Attribute
{
    public int Order { get; } = priority;
}