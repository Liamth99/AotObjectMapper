namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Specifies a post-mapping method that will be executed after the mapping
/// between the source and destination types is complete. This can be used to
/// customize or extend the behavior of the mapping operation.
/// </summary>
/// <typeparam name="TSource">The type of the source object being mapped.</typeparam>
/// <typeparam name="TDestination">The type of the destination object being mapped.</typeparam>
/// <param name="priority">
/// The execution priority of the post-mapping method. Methods with a lower
/// priority value are executed before those with a higher priority value.
/// The default value is <see cref="int.MaxValue"/>.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class PostMapAttribute<TSource, TDestination>(int priority = int.MaxValue) : Attribute
{
    /// Gets the execution priority of the post-mapping method. Methods with a lower
    /// priority value are executed before those with a higher priority value.
    public int Order { get; } = priority;
}