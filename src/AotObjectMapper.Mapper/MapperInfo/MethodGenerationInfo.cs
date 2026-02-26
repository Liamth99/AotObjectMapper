using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AotObjectMapper.Mapper.Info;

public sealed class MethodGenerationInfo
{
    /// The type of the mapper being used in code generation annotated with a <see cref="GenerateMapperAttribute"/>.
    public INamedTypeSymbol MapperType { get; }

    /// Represents the source type from which data is being mapped.
    public INamedTypeSymbol SourceType { get; }

    /// Represents the collection of readable properties from the source type used during the generation of mapping logic in a mapper.
    public IPropertySymbol[] SourceProperties { get; }

    /// The type representing the destination object in the mapping process.
    public INamedTypeSymbol DestinationType { get; }

    /// A dictionary representing the setable properties of the destination type.
    public Dictionary<string, IPropertySymbol> DestinationProperties { get; }

    /// A dictionary that maps a type to a list of types it can polymorphically convert to in the context of object mapping.
    /// This property is used to handle scenarios where source types can be mapped to multiple destination types
    /// based on their runtime or compile-time polymorphism relationships.
    public Dictionary<ITypeSymbol, List<ITypeSymbol>> PolymorphableTypes { get; }

    /// The <see cref="GenerateMapperAttribute"/> used during the mapping process to define mapping rules.
    public AttributeData MapAttribute { get; }

    /// An array of <see cref="MapAttribute{TSource, TDestination}"/> data representing mapping-related attributes applied to the method, utilized during the code generation process.
    public AttributeData[] Maps { get; }

    /// An array of <see cref="MapAttribute{TSource, TDestination}"/> representing additional mappers needed for nested or complex mapping scenarios.
    public AttributeData[] OtherMappers { get; }

    /// An array of <see cref="MapAttribute{TSource, TDestination}"/> attributes associated with the current mapping process.
    public AttributeData[] AllMaps { get; }

    /// Represents the factory method used to an instance of <see cref="DestinationType"/> within the mapper code.
    public IMethodSymbol? FactoryMethod { get; }

    public List<(ITypeSymbol source, ITypeSymbol destination)> PossibleTypeMaps { get; }

    /// A collection of methods that are defined by the user in the mapper class.
    public IMethodSymbol[] UserDefinedMapperMethods { get; }

    /// A collection of methods that are executed before property assignments during the object mapping process.
    public SymbolAttributeInfo<IMethodSymbol>[] PreMapMethods { get; }

    /// A collection of methods invoked after the mapping operation is complete.
    public SymbolAttributeInfo<IMethodSymbol>[] PostMapMethods { get; }

    /// An array containing all pre-mapping queries extracted from methods annotated with relevant attributes for use in the object mapping process.
    public SymbolAttributeInfo<IMethodSymbol>[] AllPreMapQueries { get; }

    /// Represents a collection of map method information that corresponds to post-map query operations
    /// defined in the context of a mapping process.
    public SymbolAttributeInfo<IMethodSymbol>[] AllPostMapQueries { get; }

    /// Represents a collection of user-defined mapping methods that are explicitly specified for mapping individual
    /// members during the generation of mapping logic.
    public SymbolAttributeInfo<IMethodSymbol>[] ForMemberMethods { get; }

    public string[] IgnoredMembers { get; }

    public (IPropertySymbol propertySymbol, string assignemnt)[] PropertyAssignments { get; }

    public bool AllowIConvertable            { get; }
    public bool SuppressNullWarnings         { get; }
    public bool PreserveReferences           { get; }
    public bool MapEnumsByValue              { get; }
    public bool ThrowExceptionOnUnmappedEnum { get; }


    public MethodGenerationInfo(Compilation compilation, SourceProductionContext context, ITypeSymbol mapperType, AttributeData mapAttr)
    {
        MapperType      = (INamedTypeSymbol)mapperType;
        SourceType      = (INamedTypeSymbol)mapAttr.AttributeClass!.TypeArguments[0];
        DestinationType = (INamedTypeSymbol)mapAttr.AttributeClass!.TypeArguments[1];
        MapAttribute    = mapAttr;
        Maps            = MapperType.GetAttributes().Where(attr => attr.AttributeClass?.Name == nameof(MapAttribute<,>)).ToArray();
        OtherMappers    = MapperType.GetAttributes().Where(attr => attr.AttributeClass?.Name == nameof(UseMapAttribute<,,>)).ToArray();

        AllMaps = Maps.Concat(OtherMappers.SelectMany(x => x.AttributeClass!.TypeArguments[0].GetAttributes().Where(attr => attr.AttributeClass?.Name == nameof(MapAttribute<,>)))).ToArray();

        PossibleTypeMaps = new();

        foreach (var attributeData in AllMaps)
        {
            PossibleTypeMaps.Add( new(attributeData.AttributeClass!.TypeArguments[0], attributeData.AttributeClass.TypeArguments[1]) );
        }

        PolymorphableTypes = InheritanceUtils.CreatePolymorphismMap(PossibleTypeMaps.Select(x => x.source).Concat(PossibleTypeMaps.Select(x => x.destination)));

        var mapAttributeData = mapperType.GetGenericAttribute(nameof(MapAttribute<,>), SourceType, DestinationType)!;

        int mappingOptions = Convert.ToInt32(MapperType.GetAttributes().Single(x => x.AttributeClass!.Name == nameof(GenerateMapperAttribute)).ConstructorArguments[0].Value);

        IgnoredMembers               = mapAttributeData.ConstructorArguments[0].Values.Select(x => x.Value).OfType<string>().ToArray();
        AllowIConvertable            = (mappingOptions & 1U)  > 0;
        SuppressNullWarnings         = (mappingOptions & 2U)  > 0;
        PreserveReferences           = (mappingOptions & 4U)  > 0;
        MapEnumsByValue              = (mappingOptions & 8U)  > 0;
        ThrowExceptionOnUnmappedEnum = (mappingOptions & 16U) > 0;

        SourceProperties = SourceType.GetAllReadableProperties().ToArray();
        DestinationProperties = DestinationType.GetAllReadableProperties().ToDictionary(p => p.Name);

        UserDefinedMapperMethods = mapperType.GetMembers().OfType<IMethodSymbol>().ToArray();

        PreMapMethods = UserDefinedMapperMethods.GetSymbolsWithSingleGenericAttribute(nameof(PreMapAttribute<,>), SourceType, DestinationType).ToArray();

        PostMapMethods = UserDefinedMapperMethods.GetSymbolsWithSingleGenericAttribute(nameof(PostMapAttribute<,>), SourceType, DestinationType).ToArray();

        AllPreMapQueries = UserDefinedMapperMethods.GetSymbolsWithSingleAttribute(nameof(PreMapQueryAttribute<,>)).ToArray();

        AllPostMapQueries = UserDefinedMapperMethods.GetSymbolsWithSingleAttribute(nameof(PostMapQueryAttribute<,>)).ToArray();

        FactoryMethod = UserDefinedMapperMethods.SingleOrDefault(method => method.GetGenericAttribute(nameof(UseFactoryAttribute<>), DestinationType) is not null);

        ForMemberMethods = UserDefinedMapperMethods.GetSymbolsWithSingleGenericAttribute(nameof(ForMemberAttribute<,>), SourceType, DestinationType).ToArray();

        PropertyAssignments = GeneratePropertyAssignments(compilation, context);
    }

    private (IPropertySymbol propertySymbol, string assignemnt)[] GeneratePropertyAssignments(Compilation compilation, SourceProductionContext context)
    {
        List<(IPropertySymbol propertySymbol, string assignemnt)> assignments = [];

        foreach (var srcProp in SourceProperties)
        {
            if (!DestinationProperties.TryGetValue(srcProp.Name, out var destProp))
                continue;

            if (IgnoredMembers.Any(x => x.Equals(destProp.Name)))
                continue;

            if(TryBuildAssignmentExpression(compilation, context, srcProp.Type ,destProp.Type, $"src.{srcProp.Name}", srcProp.NullableAnnotation is not NullableAnnotation.None, out var expression))
                assignments.Add(new (destProp, expression));
        }

        foreach (var mapToMethod in ForMemberMethods)
        {
            var destPropName = (string)mapToMethod.Attribute.ConstructorArguments[0].Value!;

            if(!DestinationProperties.TryGetValue(destPropName, out var destProp))
            {
                var attributeSyntax = (AttributeSyntax)mapToMethod.Attribute.ApplicationSyntaxReference!.GetSyntax();

                context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM201_MemberNamesShouldBeValid, attributeSyntax.ArgumentList!.Arguments[0].GetLocation(), destPropName, DestinationType.Name));
                continue;
            }

            bool hasError = false;

            if (!mapToMethod.Symbol.IsStatic)
            {
                foreach (var location in mapToMethod.Symbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM104_MethodHasIncorrectSignatureNotStatic, location ,mapToMethod.Symbol.Name));
                }
                hasError = true;
            }

            var sourceType = mapToMethod.Attribute.AttributeClass!.TypeArguments[0];

            if (mapToMethod.Symbol.Parameters.Length is 0)
            {
                foreach (var location in mapToMethod.Symbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM101_MethodHasIncorrectSignatureParameterType, location, "First", mapToMethod.Symbol.Name, sourceType.Name));
                }
                hasError = true;
            }

            else if (!mapToMethod.Symbol.Parameters[0].Type.Equals(sourceType, SymbolEqualityComparer.Default))
            {
                var declaration = mapToMethod
                                 .Symbol
                                 .DeclaringSyntaxReferences
                                 .Select(r => r.GetSyntax(context.CancellationToken))
                                 .OfType<MethodDeclarationSyntax>()
                                 .FirstOrDefault();

                var properties = ImmutableDictionary<string, string>.Empty
                    .Add("ExpectedType", sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

                context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM101_MethodHasIncorrectSignatureParameterType, declaration.ParameterList.Parameters[0].Type.GetLocation(), properties, "First", mapToMethod.Symbol.Name, sourceType.Name));

                hasError = true;
            }

            if(mapToMethod.Symbol.Parameters.Length > 1 && !mapToMethod.Symbol.Parameters[1].Type.ToDisplayString().Equals("AotObjectMapper.Abstractions.Models.MapperContextBase", StringComparison.InvariantCulture))
            {
                var declaration = mapToMethod
                                 .Symbol
                                 .DeclaringSyntaxReferences
                                 .Select(r => r.GetSyntax(context.CancellationToken))
                                 .OfType<MethodDeclarationSyntax>()
                                 .FirstOrDefault();

                var properties = ImmutableDictionary<string, string>.Empty
                    .Add("ExpectedType", "MapperContextBase");

                context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM101_MethodHasIncorrectSignatureParameterType, declaration.ParameterList.Parameters[1].Type.GetLocation(), properties, "Second", mapToMethod.Symbol.Name, "MapperContextBase"));

                hasError = true;
            }

            if(!destProp.Type.Equals(mapToMethod.Symbol.ReturnType, SymbolEqualityComparer.Default))
            {
                var declaration = mapToMethod
                                 .Symbol
                                 .DeclaringSyntaxReferences
                                 .Select(r => r.GetSyntax(context.CancellationToken))
                                 .OfType<MethodDeclarationSyntax>()
                                 .FirstOrDefault();

                var properties = ImmutableDictionary<string, string>.Empty
                    .Add("ExpectedReturnType", destProp.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

               context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM100_MethodHasIncorrectSignatureReturnType, declaration.ReturnType.GetLocation(), properties, mapToMethod.Symbol.Name, properties["ExpectedReturnType"]));

                hasError = true;
            }

            if(hasError)
                continue;

            assignments.Add(new (destProp, $"{mapToMethod.Symbol.Name}(src{(mapToMethod.Symbol.Parameters.Length is 2 ? ", ctx" : "")})"));
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

        return assignments.ToArray();
    }

    private bool TryBuildAssignmentExpression(Compilation compilation, SourceProductionContext context, ITypeSymbol sourceType, ITypeSymbol destinationType, string sourceExpression, bool sourceIsNullable, out string assignmentExpression)
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
            {
                if(otherMapper.AttributeClass!.TypeArguments[2].TryGetBlankTypeConstructor(this, out var ctor, out var ctorArgs))
                    assignmentExpression = $"ctx.GetOrMapObject<{otherMapper.AttributeClass!.TypeArguments[1].ToDisplayString()}, {otherMapper.AttributeClass!.TypeArguments[2].ToDisplayString()}>({sourceExpression}, ctx, static ({(ctorArgs.Any() ? string.Join(", ", ctorArgs.Select(x => $"{x.type} {x.argName}")) : "")}) => {ctor}, {otherMapper.AttributeClass!.TypeArguments[0].Name}.Utils.Populate)";

                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM207_NoConstructor, MapAttribute.ApplicationSyntaxReference!.GetSyntax().GetLocation(), otherMapper.AttributeClass!.TypeArguments[1].Name));
                    assignmentExpression = string.Empty;
                    return false;
                }
            }
            else
            {
                assignmentExpression = $"{otherMapper.AttributeClass!.TypeArguments[0].ToDisplayString()}.Map({sourceExpression}, ctx)";
            }

            return true;
        }

        // Try use other map methods
        var otherMapMethods = PossibleTypeMaps.Where(attr => attr.source.Equals(sourceType, SymbolEqualityComparer.Default) && attr.destination.Equals(destinationType, SymbolEqualityComparer.Default)).ToArray();

        if (otherMapMethods.Length > 0)
        {
            var method = otherMapMethods.First();

            if (PreserveReferences)
            {
                if(method.destination.TryGetBlankTypeConstructor(this, out var ctor, out var ctorArgs))
                    assignmentExpression = $"ctx.GetOrMapObject<global::{method.source.ToDisplayString()}, global::{method.destination.ToDisplayString()}>({sourceExpression}, ctx, static ({(ctorArgs.Any() ? string.Join(", ", ctorArgs.Select(x => $"{x.type} {x.argName}")) : "")}) => {ctor}, global::{MapperType.ToDisplayString()}.Utils.Populate)";

                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM207_NoConstructor, MapAttribute.ApplicationSyntaxReference!.GetSyntax().GetLocation(), destinationType.Name));
                    assignmentExpression = string.Empty;
                    return false;
                }
            }
            else
            {
                assignmentExpression = $"global::{MapperType.ToDisplayString()}.Map({sourceExpression}, ctx)";
            }

            return true;
        }

        // Enum -> Enum
        if (sourceType.TypeKind is TypeKind.Enum && destinationType.TypeKind is TypeKind.Enum)
        {
            if (MapEnumsByValue)
                assignmentExpression = $"(global::{destinationType.ToDisplayString()}){sourceExpression}";
            else
                assignmentExpression = $"{GeneratorUtils.EnumMapSwitchStatement(sourceExpression, sourceType, destinationType, ThrowExceptionOnUnmappedEnum)}";

            return true;
        }

        // IEnumerable
        if(TryGetEnumerableInitializationInfo(sourceType, out var sourceElementType))
            if(TryGetEnumerableInitializationInfo(destinationType, out var destinationElementType))
                if (TryBuildAssignmentExpression(compilation, context, sourceElementType, destinationElementType, "x", destinationElementType.NullableAnnotation is not NullableAnnotation.None, out var elementExpression))
                    if(TryGetEnumerationInitialization(destinationType, sourceElementType, destinationElementType, sourceExpression, elementExpression, out string selectExpression))
                    {
                        assignmentExpression = $"{selectExpression}";
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

    private bool TryGetEnumerationInitialization(ITypeSymbol destinationPropertyType, ITypeSymbol sourceElementType, ITypeSymbol destinationElementType, string sourceExpression, string assignmentExpression, out string initialization)
    {
        initialization = null!;

        var preMapQuery  = AllPreMapQueries.SingleOrDefault(x => x.Attribute.AttributeClass!.TypeArguments[0].Equals(sourceElementType,  SymbolEqualityComparer.Default) && x.Attribute.AttributeClass!.TypeArguments[1].Equals(destinationElementType, SymbolEqualityComparer.Default));
        var postMapQuery = AllPostMapQueries.SingleOrDefault(x => x.Attribute.AttributeClass!.TypeArguments[0].Equals(sourceElementType, SymbolEqualityComparer.Default) && x.Attribute.AttributeClass!.TypeArguments[1].Equals(destinationElementType, SymbolEqualityComparer.Default));

        string selectExpression;

        if (preMapQuery is not null)
            selectExpression = $"{preMapQuery.Symbol.Name}({sourceExpression}{(preMapQuery.Symbol.Parameters.Length is 2 ? ", ctx" : "")}).Select(x => {assignmentExpression})";
        else
            selectExpression = $"{sourceExpression}.Select(x => {assignmentExpression})";

        if (postMapQuery is not null)
            selectExpression = $"{postMapQuery.Symbol.Name}({selectExpression}{(postMapQuery.Symbol.Parameters.Length is 2 ? ", ctx" : "")})";

        if (destinationPropertyType.Name is "IEnumerable")
            initialization = selectExpression;

        else if (destinationPropertyType is IArrayTypeSymbol)
            initialization = $"{selectExpression}.ToArray()";

        else if (destinationPropertyType.Name is "List" or "ICollection" or "IReadOnlyList" or "IReadOnlyCollection")
            initialization = $"{selectExpression}.ToList()";

        return initialization is not null;
    }
}