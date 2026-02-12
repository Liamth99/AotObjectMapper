using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using AotObjectMapper.Abstractions.Exceptions;

namespace AotObjectMapper.Abstractions.Models;

/// <summary>
/// A base class for managing the state and context of object mapping operations.
/// This class provides mechanisms for tracking mapping depth, managing mapped objects, and enforcing depth constraints.
/// </summary>
public abstract class MapperContextBase
{
    ///
    protected string DebugString()
    {
        StringBuilder sb = new();

        sb.Append($"Depth:{Depth}");
        if (_maxDepth is not null)
            sb.Append($"/{_maxDepth}");

#if DEBUG
        sb.Append($" TotalMaps:{TotalMaps} ByReference:{TotalReferencedObjects}");
#endif

        return sb.ToString();
    }

#if DEBUG
    /// The total number of mapping operations performed within the context.
    /// Used for debugging and tracking the performance of the mapping process.
    public int TotalMaps { get; private set; }

    /// The total number of objects that are referenced during the mapping process.
    /// This property is used to track how many previously instantiated or referenced objects
    /// have been identified and reused, primarily for debugging and optimization purposes.
    public int TotalReferencedObjects { get; private set; }
#endif

    /// The current depth level of a mapping process.
    public abstract int Depth { get; }

    /// Specifies the maximum allowable depth for nested mapping operations within the context.
    public int? MaxDepth => _maxDepth;
    private readonly int? _maxDepth;

    ///
    public MapperContextBase(int? maxDepth = null)
    {
        _maxDepth = maxDepth;
    }

    /// <summary>
    /// Increments the current depth level in the mapping context.
    /// </summary>
    /// <exception cref="MaxMapDepthException">
    /// Thrown when the maximum allowable depth specified by <c>MaxDepth</c> is exceeded.
    /// </exception>
    public void IncrementDepth()
    {
        if (Depth >= MaxDepth)
            throw new MaxMapDepthException($"Max depth of {MaxDepth} exceeded while mapping.");

        _incrementDepth();
    #if DEBUG
        TotalMaps++;
    #endif
    }
    ///
    protected abstract void _incrementDepth();

    /// <summary>
    /// Decrements the current depth level in the mapping context.
    /// Used to track the depth of nested mapping operations.
    /// </summary>
    public void DecrementDepth()
    {
        _decrementDepth();

        Debug.Assert(Depth >= 0, "_depth < 0", $"Depth should never be negative but was {Depth}");
    }
    ///
    protected abstract void _decrementDepth();

    /// <summary>
    /// A dictionary used to store additional contextual information for the mapping process.
    /// </summary>
    public abstract IDictionary<string, object> AdditionalContext { get; }

    /// A collection that maintains references to objects mapped during the current mapping operation.
    protected abstract IDictionary<object, object> ExistingRefencedObjects { get; }

    /// <summary>
    /// Retrieves a mapped object from the context if it already exists, or maps the object if it is not found.
    /// </summary>
    /// <param name="source">The source object to be looked up or mapped.</param>
    /// <param name="context">The current state of the mapping context.</param>
    /// <param name="sourceInitializer">A function that initializes a new instance of the destination object if one does not already exist.</param>
    /// <param name="populateMethod">The mapping action to populate the object if one does not already exist.</param>
    /// <returns>The mapped destination object associated with the source.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TDestination GetOrMapObject<TSource, TDestination>(TSource source, MapperContextBase context, Func<TDestination> sourceInitializer, Action<TDestination, TSource, MapperContextBase> populateMethod)
        where TSource : notnull
        where TDestination : notnull
    {
        if (ExistingRefencedObjects.TryGetValue(source, out var destination))
        {
#if DEBUG
            TotalReferencedObjects++;
#endif
            return (TDestination)destination;
        }

        var newObj = sourceInitializer.Invoke();

        ExistingRefencedObjects.Add(source, newObj);

        populateMethod(newObj, source, context);

        return newObj;
    }

    /// <summary>
    /// Retrieves a mapped object from the context if it already exists, or maps the object if it is not found.
    /// </summary>
    /// <param name="source">The source object to be looked up or mapped.</param>
    /// <param name="context">The current state of the mapping context.</param>
    /// <param name="sourceInitializer">A function that initializes a new instance of the destination object if one does not already exist.</param>
    /// <param name="populateMethod">The mapping action to populate the object if one does not already exist.</param>
    /// <returns>The mapped destination object associated with the source.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TDestination GetOrMapObject<TSource, TDestination>(TSource source, MapperContextBase context, Func<MapperContextBase, TDestination> sourceInitializer, Action<TDestination, TSource, MapperContextBase> populateMethod) where TSource : notnull where TDestination : notnull
    {
        if (ExistingRefencedObjects.TryGetValue(source, out var destination))
        {
#if DEBUG
            TotalReferencedObjects++;
#endif
            return (TDestination)destination;
        }

        var newObj = sourceInitializer.Invoke(context);

        ExistingRefencedObjects.Add(source, newObj);

        populateMethod(newObj, source, context);

        return newObj;
    }
}