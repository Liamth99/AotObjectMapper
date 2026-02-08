namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// An attribute that allows the configuration or customization of post-map queries
/// between the specified source and destination types in an object mapping process.
/// </summary>
/// <typeparam name="TSource">The type of the source object being mapped.</typeparam>
/// <typeparam name="TDestination">The type of the destination object being mapped.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class PostMapQueryAttribute<TSource, TDestination> : Attribute;