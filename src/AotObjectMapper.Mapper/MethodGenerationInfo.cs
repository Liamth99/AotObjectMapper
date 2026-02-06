using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AotObjectMapper.Mapper;

public sealed class MethodGenerationInfo
{
    /// The type of the mapper being used in code generation annotated with a <see cref="GenerateMapperAttribute"/>.
    public INamedTypeSymbol MapperType { get; }

    /// Represents the source type from which data is being mapped.
    public INamedTypeSymbol SourceType { get;  }

    /// Represents the collection of readable properties from the source type used during the generation of mapping logic in a mapper.
    public IPropertySymbol[] SourceProperties { get;  }

    /// The type representing the destination object in the mapping process.
    public INamedTypeSymbol DestinationType { get;  }

    /// A dictionary representing the setable properties of the destination type.
    public Dictionary<string, IPropertySymbol> DestinationProperties { get;  }

    /// A dictionary that maps a type to a list of types it can polymorphically convert to in the context of object mapping.
    /// This property is used to handle scenarios where source types can be mapped to multiple destination types
    /// based on their runtime or compile-time polymorphism relationships.
    public Dictionary<ITypeSymbol, List<ITypeSymbol>> PolymorphableTypes { get; }


    /// Represents the namespace where the generated mapper code will be placed.
    public string Namespace  { get;  }

    /// A collection of namespaces that will be included as `using` directives in the generated mapper class.
    public List<string> Usings { get;  }

    public AttributeData[] Maps { get; }

    /// A collection of attribute data representing additional mappers needed for nested or complex mapping scenarios.
    public AttributeData[] OtherMappers { get; }

    /// Represents a collection of mapping-related attributes associated with the current mapping process.
    public AttributeData[] AllMaps { get; }

    public List<(ITypeSymbol source, ITypeSymbol destination)> PossibleTypeMaps { get; }

    /// A collection of methods that are defined by the user in the mapper class.
    public IMethodSymbol[] UserDefinedMapperMethods { get;  }

    /// A collection of methods that are executed before property assignments during the object mapping process.
    public MapMethodInfo[] PreMapMethods { get;  }

    /// A collection of methods invoked after the mapping operation is complete.
    public MapMethodInfo[] PostMapMethods { get;  }

    /// Represents a collection of user-defined mapping methods that are explicitly specified for mapping individual
    /// members during the generation of mapping logic.
    public MapMethodInfo[] ForMemberMethods { get;  }

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
        Maps            = MapperType.GetAttributes().Where(attr => attr.AttributeClass?.Name == nameof(MapAttribute<,>)).ToArray();
        OtherMappers    = MapperType.GetAttributes().Where(attr => attr.AttributeClass?.Name == nameof(UseMapAttribute<,,>)).ToArray();

        AllMaps = Maps.Concat(OtherMappers.SelectMany(x => x.AttributeClass!.TypeArguments[0].GetAttributes().Where(attr => attr.AttributeClass?.Name == nameof(MapAttribute<,>)))).ToArray();

        PossibleTypeMaps = new();

        foreach (var attributeData in AllMaps)
        {
            PossibleTypeMaps.Add( new(attributeData.AttributeClass!.TypeArguments[0], attributeData.AttributeClass.TypeArguments[1]) );
        }

        PolymorphableTypes = InheritanceUtils.CreatePolymorphismMap(PossibleTypeMaps.Select(x => x.source).Concat(PossibleTypeMaps.Select(x => x.destination)));

        // Ive done this instead of parsing it through as an argument to to make things simpler when doing nested mapping
        var mapAttributeData = mapperType
                              .GetAttributes()
                              .Single(x =>
                                          x.AttributeClass!.Name == nameof(MapAttribute<,>) &&
                                          x.AttributeClass.TypeArguments[0].Equals(sourceType, SymbolEqualityComparer.Default) &&
                                          x.AttributeClass.TypeArguments[1].Equals(destinationType, SymbolEqualityComparer.Default)
                               );

        UserDefinedMapperMethods = MapperType.GetMembers().OfType<IMethodSymbol>().ToArray();

        PreMapMethods = UserDefinedMapperMethods
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

        PostMapMethods = UserDefinedMapperMethods
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

        SourceProperties = Utils.GetAllReadableProperties(sourceType).ToArray();

        DestinationProperties = Utils.GetAllSetableProperties(DestinationType).ToDictionary(p => p.Name);

        ForMemberMethods = UserDefinedMapperMethods
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

            if (ForMemberMethods.Any(x =>
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

        foreach (var mapToMethod in ForMemberMethods)
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