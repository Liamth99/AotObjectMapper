using System.Collections.Concurrent;
using System.Diagnostics;

namespace AotObjectMapper.Abstractions.Models;

/// A thread-safe implementation of the <see cref="MapperContextBase"/> that uses
/// concurrent dictionaries to manage additional context and referenced objects during object mapping operations.
[DebuggerDisplay("{DebugString()}")]
public class ConcurrentMapperContext : MapperContextBase
{
    ///
    public ConcurrentMapperContext(int? maxDepth = null) : base(maxDepth)
    {
        AdditionalContext = new ConcurrentDictionary<string, object>();
    }

    ///
    public ConcurrentMapperContext(ConcurrentDictionary<string, object> additionalContext, int? maxDepth = null) : base(maxDepth)
    {
        AdditionalContext = additionalContext;
    }

    /// <inheritdoc/>
    protected override void _incrementDepth() => _depth.Value++;

    /// <inheritdoc/>
    protected override void _decrementDepth() => _depth.Value--;

    /// <inheritdoc/>
    public override IDictionary<string, object> AdditionalContext { get; }

    /// <inheritdoc/>
    protected override IDictionary<object, object> ExistingRefencedObjects { get; } = new ConcurrentDictionary<object, object>(ReferenceEqualityComparer.Instance);

    /// <inheritdoc/>
    public override int Depth => _depth.Value;
    private static readonly AsyncLocal<int> _depth = new();
}