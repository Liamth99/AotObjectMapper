using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AotObjectMapper.Mapper;

public sealed class MethodGenerationInfo
{
    public INamedTypeSymbol                   MapperType            { get; }
    public INamedTypeSymbol                   SourceType            { get;  }
    public IPropertySymbol[]                  SourceProperties      { get;  }
    public INamedTypeSymbol                   DestinationType       { get;  }
    public Dictionary<string,IPropertySymbol> DestinationProperties { get;  }

    public string       Namespace  { get;  }
    public List<string> Usings     { get;  }

    public AttributeData[] OtherMappers   { get;  }
    public IMethodSymbol[] MapperMethods  { get;  }
    public MapMethodInfo[] PreMapMethods { get;  }
    public MapMethodInfo[] PostMapMethods { get;  }
    public MapMethodInfo[] MapToMethods   { get;  }

    public string[] IgnoredMembers { get;  }

    public bool AllowIConvertable            {get; }
    public bool SuppressNullWarnings         {get; }
    public bool PreserveReferences           {get; }
    public bool MapEnumsByValue              {get; }
    public bool ThrowExceptionOnUnmappedEnum {get; }


    public MethodGenerationInfo(ITypeSymbol mapperType, ITypeSymbol sourceType, ITypeSymbol destinationType)
    {
        MapperType      = (INamedTypeSymbol)mapperType;
        SourceType      = (INamedTypeSymbol)sourceType;
        DestinationType = (INamedTypeSymbol)destinationType;
        Namespace       = MapperType.ContainingNamespace.ToDisplayString();
        OtherMappers    = MapperType.GetAttributes().Where(attr => attr.AttributeClass?.Name == nameof(UseMapAttribute<,,>)).ToArray();

        // Ive done this instead of parsing it through as an argument to to make things simpler when doing nested mapping
        var mapAttributeData = mapperType
                              .GetAttributes()
                              .Single(x =>
                                          x.AttributeClass!.Name == nameof(MapAttribute<,>) &&
                                          x.AttributeClass.TypeArguments[0].Equals(sourceType, SymbolEqualityComparer.Default) &&
                                          x.AttributeClass.TypeArguments[1].Equals(destinationType, SymbolEqualityComparer.Default)
                               );

        MapperMethods = MapperType.GetMembers().OfType<IMethodSymbol>().ToArray();

        PreMapMethods = MapperMethods
                       .Select(method =>
                        {
                            var attributes = method.GetAttributes();
                            var attribute = attributes.SingleOrDefault(
                                attr => attr.AttributeClass?.Name == nameof(PreMapAttribute<,>) &&
                                        attr.AttributeClass!.TypeArguments[0].Equals(SourceType, SymbolEqualityComparer.Default) &&
                                        attr.AttributeClass!.TypeArguments[1].Equals(DestinationType, SymbolEqualityComparer.Default));

                            if (attribute is null)
                                return new MapMethodInfo(null!, null!);

                            return new MapMethodInfo(method, attribute);
                        })
                       .Where(x => x.Method is not null) // Required check as above we suppress the null warning
                       .ToArray();

        PostMapMethods = MapperMethods
                            .Select(method =>
                             {
                                 var attributes = method.GetAttributes();
                                 var attribute = attributes.SingleOrDefault(
                                     attr => attr.AttributeClass?.Name == nameof(PostMapAttribute<,>) &&
                                             attr.AttributeClass!.TypeArguments[0].Equals(SourceType, SymbolEqualityComparer.Default) &&
                                             attr.AttributeClass!.TypeArguments[1].Equals(DestinationType, SymbolEqualityComparer.Default));

                                 if (attribute is null)
                                     return new MapMethodInfo(null!, null!);

                                 return new MapMethodInfo(method, attribute);
                             })
                            .Where(x => x.Method is not null) // Required check as above we suppress the null warning
                            .ToArray();

        int mappingOptions = Convert.ToInt32(MapperType.GetAttributes().Single(x => x.AttributeClass!.Name == nameof(GenerateMapperAttribute)).ConstructorArguments[0].Value);

        IgnoredMembers               = mapAttributeData.ConstructorArguments[0].Values.Select(x => x.Value).OfType<string>().ToArray();
        AllowIConvertable            = (mappingOptions & 1U)  > 0;
        SuppressNullWarnings         = (mappingOptions & 2U)  > 0;
        PreserveReferences           = (mappingOptions & 4U)  > 0;
        MapEnumsByValue              = (mappingOptions & 8U)  > 0;
        ThrowExceptionOnUnmappedEnum = (mappingOptions & 16U) > 0;

        var format = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

        Usings = ["System", "System.ComponentModel", "System.Diagnostics.Contracts", "AotObjectMapper.Abstractions.Models", SourceType.ContainingNamespace!.ToDisplayString(format), destinationType.ContainingNamespace!.ToDisplayString(format)];
        
        SourceProperties = SourceType.GetMembers()
                              .OfType<IPropertySymbol>()
                              .Where(p => p.GetMethod is not null)
                              .ToArray();

        DestinationProperties = DestinationType.GetMembers()
                            .OfType<IPropertySymbol>()
                            .Where(p => p.SetMethod is not null)
                            .ToDictionary(p => p.Name);

        MapToMethods = MapperMethods
                               .Select(method =>
                                {
                                    var attributes = method.GetAttributes();
                                    var attribute = attributes.SingleOrDefault(
                                        attr => attr.AttributeClass?.Name == nameof(ForMemberAttribute<,>) &&
                                                attr.AttributeClass!.TypeArguments[0].Equals(SourceType, SymbolEqualityComparer.Default) &&
                                                attr.AttributeClass!.TypeArguments[1].Equals(DestinationType, SymbolEqualityComparer.Default));

                                    if (attribute is null)
                                        return new MapMethodInfo(null!, null!);

                                    return new MapMethodInfo(method, attribute);
                                })
                               .Where(x => x.Method is not null) // Required check as above we suppress the null warning
                               .ToArray();
    }

    public IEnumerable<(IPropertySymbol propertySymbol, string assignemnt)> GeneratePropertyAssignments(Compilation compilation)
    {
        List<(IPropertySymbol propertySymbol, string assignemnt)> assignments = [];

        foreach (var srcProp in SourceProperties)
        {
            if (!DestinationProperties.TryGetValue(srcProp.Name, out var destProp))
                continue;

            if (IgnoredMembers.Any(x => x.Equals(destProp.Name)))
                continue;

            var otherMapper = OtherMappers.FirstOrDefault(
                attr => attr.AttributeClass?.TypeArguments[1].Equals(srcProp.Type, SymbolEqualityComparer.Default) is true &&
                        attr.AttributeClass.TypeArguments[2].Equals(destProp.Type, SymbolEqualityComparer.Default));

            if (otherMapper is not null)
            {
                if (PreserveReferences)
                {
                    assignments.Add(new(destProp, $"context.GetOrMapObject<{otherMapper.AttributeClass!.TypeArguments[1].ToDisplayString()}, {otherMapper.AttributeClass!.TypeArguments[2].ToDisplayString()}>(source.{srcProp.Name}, context, static () => {Utils.BlankTypeConstructor(otherMapper.AttributeClass!.TypeArguments[2])}, {otherMapper.AttributeClass!.TypeArguments[0].Name}.{otherMapper.AttributeClass!.TypeArguments[2].Name}_Utils.Populate)"));
                }
                else
                    assignments.Add(new(destProp, $"{otherMapper.AttributeClass!.TypeArguments[0].ToDisplayString()}.Map(source.{srcProp.Name}, context)"));

                continue;
            }

            if (MapToMethods.Any(x =>
                {
                    var attCtorArguments = x.Attribute.ConstructorArguments;
                    return attCtorArguments[0].Value!.Equals(destProp.Name);
                }))
                continue; // Dont want to double map properties with explicit Map to methods

            if (!SymbolEqualityComparer.Default.Equals(srcProp.Type, destProp.Type))
            {
                if (srcProp.Type.TypeKind is TypeKind.Enum && destProp.Type.TypeKind is TypeKind.Enum)
                {
                    if (MapEnumsByValue)
                        assignments.Add(new(destProp, $"({destProp.Type.ToDisplayString()})source.{srcProp.Name}"));
                    else
                        assignments.Add(new(destProp, $"{Utils.EnumMapSwitchStatement($"source.{destProp.Name}", srcProp.Type, destProp.Type, ThrowExceptionOnUnmappedEnum)}"));

                    continue;
                }

                if (!AllowIConvertable)
                    continue;

                if (!Utils.TryGetConvertibleInfo(destProp.Type, compilation, out var destCanBeNull, out var destUnderlyingType))
                    continue;

                if (!Utils.ConvertMethods.TryGetValue(destUnderlyingType?.SpecialType ?? destProp.Type.SpecialType, out var method))
                    continue;

                if (Utils.TryGetConvertibleInfo(srcProp.Type, compilation, out var canBeNull, out _))
                {
                    var formatProviderExpr = Utils.GetFormatProviderExpression(MapperType, compilation, (srcProp.Type as INamedTypeSymbol)!, (destProp.Type as INamedTypeSymbol)!);
                    var convertArgsSuffix  = formatProviderExpr is null ? "" : $", {formatProviderExpr}";

                    if (canBeNull && SuppressNullWarnings)
                    {
                        if (destCanBeNull)
                            assignments.Add(new(destProp, $"source.{srcProp.Name} is null ? null : Convert.{method}(source.{srcProp.Name}{convertArgsSuffix})"));
                        else
                            assignments.Add(new(destProp, $"source.{srcProp.Name} is null ? default! : Convert.{method}(source.{srcProp.Name}{convertArgsSuffix})"));
                    }
                    else
                        assignments.Add(new(destProp, $"Convert.{method}(source.{srcProp.Name}{convertArgsSuffix})"));
                }
                else
                    continue;
            }
            else
            {
                if (Utils.IsSimpleType(destProp.Type))
                    assignments.Add(new(destProp, $"source.{srcProp.Name}{(srcProp.NullableAnnotation == NullableAnnotation.Annotated && SuppressNullWarnings ? " ?? default!" : "")}"));
            }
        }

        foreach (var mapToMethod in MapToMethods)
        {
            if(!DestinationProperties.TryGetValue((string)mapToMethod.Attribute.ConstructorArguments[0].Value!, out var destProp))
                continue;

            assignments.Add(new (destProp, $"{mapToMethod.Method.Name}(source{(mapToMethod.Method.Parameters.Length is 2 ? ", context" : "")})"));
        }

        if (SuppressNullWarnings)
        {
            foreach (var destProp in DestinationProperties.Values)
            {
                if (!destProp.IsRequired)
                    continue;

                if (assignments.Any(x => x.propertySymbol.Equals(destProp, SymbolEqualityComparer.Default)))
                    continue;

                assignments.Add(new (destProp, "null!"));
            }
        }

        return assignments;
    }
}

public sealed class MapMethodInfo(IMethodSymbol method, AttributeData attribute)
{
    public IMethodSymbol Method    { get; } = method;
    public AttributeData Attribute { get; } = attribute;
}