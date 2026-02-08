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

        Usings = ["System", "System.Collections.Generic", "System.Linq", "System.ComponentModel", "System.Diagnostics.Contracts", "AotObjectMapper.Abstractions.Models", SourceType.ContainingNamespace!.ToDisplayString(format), destinationType.ContainingNamespace!.ToDisplayString(format)];

        SourceProperties = sourceType.GetAllReadableProperties().ToArray();

        DestinationProperties = DestinationType.GetAllReadableProperties().ToDictionary(p => p.Name);

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

            if(TryBuildAssignmentExpression(srcProp.Type ,destProp.Type, $"src.{srcProp.Name}", srcProp.NullableAnnotation is not NullableAnnotation.None, compilation, out var expression))
                assignments.Add(new (destProp, expression));
        }

        foreach (var mapToMethod in ForMemberMethods)
        {
            if(!DestinationProperties.TryGetValue((string)mapToMethod.Attribute.ConstructorArguments[0].Value!, out var destProp))
                continue;

            assignments.Add(new (destProp, $"{mapToMethod.Method.Name}(src{(mapToMethod.Method.Parameters.Length is 2 ? ", ctx" : "")})"));
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

    private bool TryBuildAssignmentExpression(ITypeSymbol sourceType, ITypeSymbol destinationType, string sourceExpression, bool sourceIsNullable, Compilation compilation, out string assignmentExpression)
    {
        // exact type match
        if (sourceType.Equals(destinationType, SymbolEqualityComparer.Default))
        {
            assignmentExpression = $"{sourceExpression}{(sourceIsNullable && SuppressNullWarnings ? " ?? default!" : "")}";

            return true;
        }

        // Try use other mapper class
        var otherMapper = OtherMappers.FirstOrDefault(
            attr => attr.AttributeClass?.TypeArguments[1].Equals(sourceType, SymbolEqualityComparer.Default) is true &&
                    attr.AttributeClass.TypeArguments[2].Equals(destinationType, SymbolEqualityComparer.Default));

        if (otherMapper is not null)
        {
            if (PreserveReferences)
                assignmentExpression = $"ctx.GetOrMapObject<{otherMapper.AttributeClass!.TypeArguments[1].ToDisplayString()}, {otherMapper.AttributeClass!.TypeArguments[2].ToDisplayString()}>({sourceExpression}, ctx, static () => {otherMapper.AttributeClass!.TypeArguments[2].BlankTypeConstructor()}, {otherMapper.AttributeClass!.TypeArguments[0].Name}.{otherMapper.AttributeClass!.TypeArguments[2].Name}_Utils.Populate)";
            else
                assignmentExpression = $"{otherMapper.AttributeClass!.TypeArguments[0].ToDisplayString()}.Map({sourceExpression}, ctx)";

            return true;
        }

        // Try use other map methods
        var otherMapMethods = PossibleTypeMaps.Where(attr => attr.source.Equals(sourceType, SymbolEqualityComparer.Default) && attr.destination.Equals(destinationType, SymbolEqualityComparer.Default)).ToArray();

        if (otherMapMethods.Length > 0)
        {
            var method = otherMapMethods.First();

            if (PreserveReferences)
                assignmentExpression = $"ctx.GetOrMapObject<{method.source.ToDisplayString()}, {method.destination.ToDisplayString()}>({sourceExpression}, ctx, static () => {method.destination.BlankTypeConstructor()}, {MapperType.ToDisplayString()}.{method.destination.Name}_Utils.Populate)";
            else
                assignmentExpression = $"{MapperType.ToDisplayString()}.Map({sourceExpression}, ctx)";

            return true;
        }

        // Enum -> Enum
        if (sourceType.TypeKind is TypeKind.Enum && destinationType.TypeKind is TypeKind.Enum)
        {
            if (MapEnumsByValue)
                assignmentExpression = $"({destinationType.ToDisplayString()}){sourceExpression}";
            else
                assignmentExpression = $"{GeneratorUtils.EnumMapSwitchStatement(sourceExpression, sourceType, destinationType, ThrowExceptionOnUnmappedEnum)}";

            return true;
        }

        // IEnumerable
        if(TryGetEnumerableInitializationInfo(sourceType, out var sourceElementType))
            if(TryGetEnumerableInitializationInfo(destinationType, out var destinationElementType))
                if (TryBuildAssignmentExpression(sourceElementType, destinationElementType, "x", destinationElementType.NullableAnnotation is not NullableAnnotation.None, compilation, out var elementExpression))
                    if(TryGetEnumerationInitialization(destinationType, elementExpression, out string selectExpression))
                    {
                        assignmentExpression = $"{sourceExpression}{selectExpression}";
                        return true;
                    }

        // IConvertable
        if (AllowIConvertable &&
            InheritanceUtils.TryGetConvertibleInfo(destinationType, compilation, out var destCanBeNull, out var destUnderlyingType) &&
            GeneratorUtils.ConvertMethods.TryGetValue(destUnderlyingType?.SpecialType ?? destinationType.SpecialType, out var convertMethod) &&
            InheritanceUtils.TryGetConvertibleInfo(sourceType, compilation, out var canBeNull, out _))
        {
            var formatProviderExpr = GeneratorUtils.GetFormatProviderExpression(MapperType, compilation, (INamedTypeSymbol)sourceType, (INamedTypeSymbol)destinationType);
            var convertArgsSuffix  = formatProviderExpr is null ? "" : $", {formatProviderExpr}";

            if (canBeNull && SuppressNullWarnings)
            {
                if (destCanBeNull)
                    assignmentExpression = $"{sourceExpression} is null ? null : Convert.{convertMethod}({sourceExpression}{convertArgsSuffix})";
                else
                    assignmentExpression = $"{sourceExpression} is null ? default! : Convert.{convertMethod}({sourceExpression}{convertArgsSuffix})";
            }
            else
                assignmentExpression = $"Convert.{convertMethod}({sourceExpression}{convertArgsSuffix})";

            return true;
        }

        assignmentExpression = null!;
        return false;
    }

    private bool TryGetEnumerableInitializationInfo(ITypeSymbol propertyType, out ITypeSymbol elementType)
    {
        elementType = null!;

        if (propertyType.OriginalDefinition.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T)
        {
            elementType = ((INamedTypeSymbol)propertyType).TypeArguments[0];
            return true;
        }

        // Ignore string types, we don't want IEnumerable<char>
        if (propertyType.SpecialType is SpecialType.System_String)
            return false;

        if (propertyType is IArrayTypeSymbol arrayType)
        {
            elementType = arrayType.ElementType;
            return true;
        }

        foreach (var iface in propertyType.AllInterfaces)
        {
            if (iface.OriginalDefinition.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T)
            {
                elementType = iface.TypeArguments[0];
                return true;
            }
        }

        return false;
    }

    private bool TryGetEnumerationInitialization(ITypeSymbol propertyType, string assignmentExpression, out string initialization)
    {
        initialization = null!;

        if (propertyType.Name is "IEnumerable")
            initialization = $".Select(x => {assignmentExpression})";

        else if (propertyType is IArrayTypeSymbol)
            initialization = $".Select(x => {assignmentExpression}).ToArray()";

        else if (propertyType.Name is "List" or "ICollection" or "IReadOnlyList" or "IReadOnlyCollection")
            initialization = $".Select(x => {assignmentExpression}).ToList()";

        return initialization is not null;
    }
}

public sealed class MapMethodInfo(IMethodSymbol method, AttributeData attribute)
{
    public IMethodSymbol Method    { get; } = method;
    public AttributeData Attribute { get; } = attribute;
}