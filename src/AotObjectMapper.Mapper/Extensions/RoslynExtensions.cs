using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AotObjectMapper.Mapper.Extensions;

public static class RoslynExtensions
{
    public static AttributeData? GetGenericAttribute(this ISymbol symbol, string attributeName, params ITypeSymbol[] genericTypes)
    {
        return symbol.GetAttributes().SingleOrDefault(
            attr =>
            {
                if (attr.AttributeClass?.Name != attributeName)
                    return false;

                if (attr.AttributeClass.TypeArguments.Length != genericTypes.Length)
                    return false;

                return attr.AttributeClass.TypeArguments.ToArray().SequenceEqual(genericTypes, SymbolEqualityComparer.Default);
            }
        );
    }

    public static IEnumerable<SymbolAttributeInfo<TSymbol>> GetSymbolsWithSingleAttribute<TSymbol>(this IEnumerable<TSymbol> symbols, string attributeName) where TSymbol : ISymbol
    {
        return symbols
              .Select(method =>
              {
                  var attribute = method.GetAttributes().SingleOrDefault(attr => attr.AttributeClass?.Name == attributeName);

                  if (attribute is null)
                      return null;

                  return new SymbolAttributeInfo<TSymbol>(method, attribute);
              })
              .OfType<SymbolAttributeInfo<TSymbol>>();
    }

    public static IEnumerable<SymbolAttributeInfo<TSymbol>> GetSymbolsWithSingleGenericAttribute<TSymbol>(this IEnumerable<TSymbol> symbols, string attributeName, params ITypeSymbol[] genericTypes) where TSymbol : ISymbol
    {
        return symbols
              .Select(method =>
              {
                  var attribute = method.GetGenericAttribute(attributeName, genericTypes);

                  if (attribute is null)
                      return null;

                  return new SymbolAttributeInfo<TSymbol>(method, attribute);
              })
              .OfType<SymbolAttributeInfo<TSymbol>>();
    }
}

public sealed class SymbolAttributeInfo<TSymbol>(TSymbol symbol, AttributeData attribute) where TSymbol : ISymbol
{
    public TSymbol       Symbol    { get; } = symbol;
    public AttributeData Attribute { get; } = attribute;
}