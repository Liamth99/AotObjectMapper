namespace AotObjectMapper.Abstractions.Models;

/// <summary>
/// A marker for mapping objects of type <typeparamref name="TSource"/> to objects of type <typeparamref name="TDestination"/>.
/// </summary>
/// <typeparam name="TSource">The type of the source object.</typeparam>
/// <typeparam name="TDestination">The type of the destination object.</typeparam>
public interface IMapper<TSource, TDestination>;