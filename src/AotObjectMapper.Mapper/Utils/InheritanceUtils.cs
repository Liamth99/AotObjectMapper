using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AotObjectMapper.Mapper.Utils;

public static class InheritanceUtils
{
    public static Dictionary<ITypeSymbol, List<ITypeSymbol>> CreatePolymorphismMap(IEnumerable<ITypeSymbol> types)
    {
        Dictionary<ITypeSymbol, List<ITypeSymbol>> result = new (SymbolEqualityComparer.Default);

        foreach (var type in types)
        {
            // Base class
            if (type.BaseType is not null && type.BaseType.SpecialType != SpecialType.System_Object)
            {
                if(result.TryGetValue(type.BaseType, out var list))
                    list.Add(type);
                else
                    result.Add(type.BaseType, [type]);
            }

            // Interfaces
            foreach (var iface in type.Interfaces)
            {
                if(result.TryGetValue(iface, out var list))
                    list.Add(type);
                else
                    result.Add(iface, [type]);
            }
        }

        return result;
    }

    public static bool TryGetConvertibleInfo(ITypeSymbol type, Compilation compilation, out bool canBeNull, out ITypeSymbol? underlyingType)
    {
        canBeNull      = false;
        underlyingType = null;

        var iConvertible = compilation.GetTypeByMetadataName("System.IConvertible");

        if (iConvertible is null)
            return false;

        // Nullable<T>
        if (type is INamedTypeSymbol named && named.OriginalDefinition.SpecialType is SpecialType.System_Nullable_T)
        {
            underlyingType = named.TypeArguments[0];
            canBeNull      = true;
            return underlyingType.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, iConvertible));
        }

        // Nullable reference type (T?)
        if (type.IsReferenceType)
        {
            canBeNull = type.NullableAnnotation == NullableAnnotation.Annotated;

            return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, iConvertible));
        }

        // Non-nullable value type
        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, iConvertible));
    }
}