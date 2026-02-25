using System.Diagnostics;

namespace AotObjectMapper.Abstractions.Models;

/// A context used for object mapping, inheriting from the base mapping context class.
[DebuggerDisplay("{DebugString()}")]
public class MapperContext : MapperContextBase
{
    ///
    public MapperContext(int? maxDepth = null) : base(maxDepth)
    {
        AdditionalContext = new Dictionary<string, object>();
    }

    ///
    public MapperContext(Dictionary<string, object> additionalContext, int? maxDepth = null) : base(maxDepth)
    {
        AdditionalContext = additionalContext;
    }

    /// <inheritdoc/>
    protected override void _incrementDepth() => _depth++;

    /// <inheritdoc/>
    protected override void _decrementDepth() => _depth--;

    /// <inheritdoc/>
    public override IDictionary<string, object> AdditionalContext { get; }

    /// <inheritdoc/>
    protected override IDictionary<object, object> ExistingRefencedObjects { get; } = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);

    /// <inheritdoc/>
    public override int Depth => _depth;
    private int _depth;
}