using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using AotObjectMapper.Abstractions.Exceptions;

namespace AotObjectMapper.Abstractions.Models;

[DebuggerDisplay("{DebugString()}")]
public class MapperContext
{

#if DEBUG
    public int TotalMaps { get; set; }
    public int TotalReferencedObjects { get; set; }
#endif

    private string DebugString()
    {
        StringBuilder sb = new();

        sb.Append($"Depth:{_depth}");
        if (_maxDepth is not null)
            sb.Append($"/{_maxDepth}");

#if DEBUG
        sb.Append($" TotalMaps:{TotalMaps} ByReference:{TotalReferencedObjects}");
#endif

        return sb.ToString();
    }

    public MapperContext(int? maxDepth = null)
    {
        _maxDepth = maxDepth;
    }

    /// <summary>
    /// Represents the current depth level of a mapping process.
    /// </summary>
    public int Depth => _depth;
    private int _depth;

    /// <summary>
    /// Specifies the maximum allowable depth for nested mapping operations within the context.
    /// </summary>
    public int? MaxDepth => _maxDepth;
    private readonly int? _maxDepth;

    /// <summary>
    /// Increments the current depth level in the mapping context.
    /// </summary>
    /// <exception cref="MaxMapDepthException">
    /// Thrown when the maximum allowable depth specified by <c>MaxDepth</c> is exceeded.
    /// </exception>
    public void IncrementDepth()
    {
        if (MaxDepth > Depth)
            throw new MaxMapDepthException($"Max depth of {MaxDepth} exceeded while mapping.");

        _depth++;
    #if DEBUG
        TotalMaps++;
    #endif
    }

    /// <summary>
    /// Decrements the current depth level in the mapping context.
    /// Used to track the depth of nested mapping operations.
    /// </summary>
    public void DecrementDepth()
    {
        _depth--;

        Debug.Assert(_depth >= 0, "_depth >= 0", "Depth should never be negative");
    }

    /// <summary>
    /// A dictionary used to store additional contextual information for the mapping process.
    /// </summary>
    public readonly Dictionary<string, object> AdditionalContext = new ();

    private readonly Dictionary<object, object> _existingRefencedObjects = new (ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Retrieves a mapped object from the context if it already exists, or maps the object if it is not found.
    /// </summary>
    /// <param name="source">The source object to be looked up or mapped.</param>
    /// <param name="context">The current state of the mapping context.</param>
    /// <param name="sourceInitializer">A function that initializes a new instance of the destination object if one does not already exist.</param>
    /// <param name="populateMethod">The mapping action to populate the object if one does not already exist.</param>
    /// <returns>The mapped destination object associated with the source.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TDestination GetOrMapObject<TSource, TDestination>(TSource source, MapperContext context, Func<TDestination> sourceInitializer, Action<TDestination, TSource, MapperContext> populateMethod)
        where TSource : notnull
        where TDestination : notnull
    {
        if (_existingRefencedObjects.TryGetValue(source, out var destination))
        {
#if DEBUG
            TotalReferencedObjects++;
#endif
            return (TDestination)destination;
        }

        var newObj = sourceInitializer.Invoke();

        _existingRefencedObjects.Add(source, newObj);

        populateMethod(newObj, source, context);

        return newObj;
    }
}