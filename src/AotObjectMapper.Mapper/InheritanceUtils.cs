using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace AotObjectMapper.Mapper;

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
}