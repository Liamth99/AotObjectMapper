using System.Collections;
using System.Runtime.CompilerServices;

namespace AotObjectMapper.Abstractions.Models;

#pragma warning disable LOCAT001
// Based off System.Collections.Generic.ReferenceEqualityComparer
internal sealed class ReferenceEqualityComparer : IEqualityComparer, IEqualityComparer<object>
{
    private ReferenceEqualityComparer() { }

    internal static readonly ReferenceEqualityComparer Instance = new();

    public new bool Equals(object? x, object? y)
    {
        return ReferenceEquals(x, y);
    }

    public int GetHashCode(object obj)
    {
        return RuntimeHelpers.GetHashCode(obj);
    }
}