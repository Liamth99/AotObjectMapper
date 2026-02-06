using AotObjectMapper.Abstractions.Models;

namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Indicates that a specific mapping method between a source and destination type should be used within a mapping generator.
/// This attribute can be applied to a class and supports configuration for mapping.
/// </summary>
/// <typeparam name="TMapGenerator">
/// The type of the mapping generator responsible for handling the mapping logic.
/// </typeparam>
/// <typeparam name="TSource">
/// The source type from which the mapping originates.
/// </typeparam>
/// <typeparam name="TDestination">
/// The destination type to which the mapping applies.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class UseMapAttribute<TMapGenerator, TSource, TDestination> : Attribute
    where TMapGenerator : IMapper<TSource, TDestination>;