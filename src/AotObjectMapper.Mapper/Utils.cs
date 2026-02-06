using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace AotObjectMapper.Mapper;

public static class Utils
{
    public static readonly ReadOnlyDictionary<SpecialType, string> ConvertMethods =
        new(new Dictionary<SpecialType, string>
        {
            [SpecialType.System_Boolean]  = "ToBoolean",
            [SpecialType.System_Byte]     = "ToByte",
            [SpecialType.System_SByte]    = "ToSByte",
            [SpecialType.System_Int16]    = "ToInt16",
            [SpecialType.System_UInt16]   = "ToUInt16",
            [SpecialType.System_Int32]    = "ToInt32",
            [SpecialType.System_UInt32]   = "ToUInt32",
            [SpecialType.System_Int64]    = "ToInt64",
            [SpecialType.System_UInt64]   = "ToUInt64",
            [SpecialType.System_Single]   = "ToSingle",
            [SpecialType.System_Double]   = "ToDouble",
            [SpecialType.System_Decimal]  = "ToDecimal",
            [SpecialType.System_Char]     = "ToChar",
            [SpecialType.System_String]   = "ToString",
            [SpecialType.System_DateTime] = "ToDateTime"
        });

    public static string BlankTypeConstructor(ITypeSymbol type)
        => $"new {type.ToDisplayString()}() {{ {string.Join(" ", type.GetMembers().OfType<IPropertySymbol>().Where(p => p.SetMethod is not null && p.IsRequired).Select(x => $"{x.Name} = {(x.Type.IsReferenceType ? "null!" : "default")},"))} }}";

    public static string EnumMapSwitchStatement(string sourceObjectName, ITypeSymbol source, ITypeSymbol destination, bool throwIfInvalid)
    {
        StringBuilder sb = new();

        sb.Append($"{sourceObjectName} switch {{ ");
        foreach (var fieldSymbol in source.GetMembers().OfType<IFieldSymbol>().Where(x => x.IsConst))
        {
            var destinationField = destination.GetMembers().OfType<IFieldSymbol>().SingleOrDefault(x => x.Name == fieldSymbol.Name);

            if (destinationField is not null)
                sb.Append($"{source.ToDisplayString()}.{fieldSymbol.Name} => {destination.ToDisplayString()}.{destinationField.Name}, ");
        }

        if(throwIfInvalid)
            sb.Append($"_ =>  throw new AotObjectMapper.Abstractions.Exceptions.EnumMissingFieldException($\"Could not map `{source.Name}` to `{destination.Name}` as `{destination.Name}` does not contain field `{{{sourceObjectName}}}`.\")");
        else
            sb.Append("_ => default");

        sb.Append(" }");

        return sb.ToString();
    }

    public static string NoInstanceTypeMapSwitchStatement(string sourceObjectName, MethodGenerationInfo info)
    {
        if (!info.PolymorphableTypes.TryGetValue(info.SourceType, out var sourceTypes) || !sourceTypes.Any())
            return string.Empty;

        StringBuilder sb = new();

        sb.Append($"{sourceObjectName} switch {{");

        int i = 0;

        foreach (var sourceType in sourceTypes)
        {
            foreach (var map in info.Maps)
            {
                if (map.AttributeClass!.TypeArguments[0].Equals(sourceType, SymbolEqualityComparer.Default))
                {
                    sb.Append($" {sourceType.ToDisplayString()} t{i} => Map(t{i}, context),");
                    i++;
                    break;
                }
            }

            foreach (var mapper in info.OtherMappers)
            {
                if (mapper.AttributeClass!.TypeArguments[1].Equals(sourceType, SymbolEqualityComparer.Default))
                {
                    sb.Append($" {sourceType.ToDisplayString()} t{i} => {mapper.AttributeClass!.TypeArguments[0].ToDisplayString()}.Map(t{i}, context),");
                    i++;
                    break;
                }
            }
        }

        sb.Append($"_ => throw new AotObjectMapper.Abstractions.Exceptions.UnhandledPolymorphicTypeException(\"Could not map type `{info.SourceType.ToDisplayString()}` - no matching destination type found.\")");

        sb.Append(" }");

        return sb.ToString();
    }

    public static string InstanceTypeMapSwitchStatement(string sourceObjectName, MethodGenerationInfo info)
    {
        if (!info.PolymorphableTypes.TryGetValue(info.SourceType, out var sourceTypes) || !sourceTypes.Any())
            return string.Empty;

        StringBuilder sb = new();

        int i = 0;

        foreach (var sourceType in sourceTypes)
        {
            foreach (var map in info.Maps)
            {
                if (map.AttributeClass!.TypeArguments[0].Equals(sourceType, SymbolEqualityComparer.Default))
                {
                    sb.AppendLine($"if ({sourceObjectName} is {sourceType.ToDisplayString()} t{i}) return Map(t{i}, context);");
                    i++;
                    break;
                }
            }

            foreach (var mapper in info.OtherMappers)
            {
                if (mapper.AttributeClass!.TypeArguments[1].Equals(sourceType, SymbolEqualityComparer.Default))
                {
                    sb.AppendLine($"if ({sourceObjectName} is {sourceType.ToDisplayString()} t{i}) return {mapper.AttributeClass!.TypeArguments[0].ToDisplayString()}.Map(t{i}, context);");
                    i++;
                    break;
                }
            }
        }

        return sb.ToString();
    }

    public static bool IsSimpleType(ITypeSymbol type)
    {
        // unwrap nullable
        if (type is INamedTypeSymbol named &&
            named.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
        {
            type = named.TypeArguments[0];
        }

        if (type.TypeKind == TypeKind.Enum)
            return true;

        if (type.SpecialType is
            SpecialType.System_Boolean or
            SpecialType.System_Byte or SpecialType.System_SByte or
            SpecialType.System_Int16 or SpecialType.System_UInt16 or
            SpecialType.System_Int32 or SpecialType.System_UInt32 or
            SpecialType.System_Int64 or SpecialType.System_UInt64 or
            SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal or
            SpecialType.System_Char or SpecialType.System_String or SpecialType.System_DateTime)
            return true;

        if (type is INamedTypeSymbol nts && nts.ContainingNamespace.ToDisplayString() == "System")
        {
            if (nts.Name is "Guid" or "DateOnly" or "TimeOnly" or "DateTimeOffset" or "TimeSpan")
                return true;
        }

        return false;
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

    public static string? GetFormatProviderExpression(INamedTypeSymbol mapper, Compilation compilation, INamedTypeSymbol sourceType, INamedTypeSymbol destinationType)
    {
        ITypeSymbol? srcEffective;
        ITypeSymbol? destEffective;

        if (sourceType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T && sourceType.TypeArguments.Length == 1)
            srcEffective = sourceType.TypeArguments[0];
        else
            srcEffective = sourceType;

        if (destinationType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T && destinationType.TypeArguments.Length == 1)
            destEffective = destinationType.TypeArguments[0];
        else
            destEffective = destinationType;

        var iFormatProvider = compilation.GetTypeByMetadataName("System.IFormatProvider");

        if (iFormatProvider is null)
            return null;

        string? defaultProviderPropertyName = null;

        foreach (var prop in mapper.GetMembers().OfType<IPropertySymbol>())
        {
            if (!prop.IsStatic)
                continue;

            if (!prop.Type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, iFormatProvider)) && !SymbolEqualityComparer.Default.Equals(prop.Type, iFormatProvider))
            {
                continue;
            }

            foreach (var attr in prop.GetAttributes())
            {
                if (attr.AttributeClass is null)
                    continue;

                // [UseFormatProvider] => default
                if (attr.AttributeClass.Name == nameof(UseFormatProviderAttribute) && attr.AttributeClass.TypeArguments.Length is 0)
                {
                    defaultProviderPropertyName ??= prop.Name;
                    continue;
                }

                // [UseFormatProvider<TSource, TDestination>] => override for specific conversion
                if (attr.AttributeClass.Name == nameof(UseFormatProviderAttribute<,>) && attr.AttributeClass.TypeArguments.Length is 2)
                {
                    var src = attr.AttributeClass.TypeArguments[0];
                    var dst = attr.AttributeClass.TypeArguments[1];

                    if (SymbolEqualityComparer.Default.Equals(src, srcEffective) && SymbolEqualityComparer.Default.Equals(dst, destEffective))
                    {
                        return prop.Name;
                    }
                }
            }
        }

        return defaultProviderPropertyName;
    }

    public static IEnumerable<IPropertySymbol> GetAllReadableProperties(ITypeSymbol type)
    {
        var seen = new HashSet<IPropertySymbol>(SymbolEqualityComparer.Default);

        for (var current = type; current is not null; current = current.BaseType)
        {
            foreach (var property in current.GetMembers().OfType<IPropertySymbol>())
            {
                if (property.GetMethod is null)
                    continue;

                if (!seen.Add(property))
                    continue;

                yield return property;
            }
        }
    }

    public static IEnumerable<IPropertySymbol> GetAllSetableProperties(ITypeSymbol type)
    {
        var seen = new HashSet<IPropertySymbol>(SymbolEqualityComparer.Default);

        for (var current = type; current is not null; current = current.BaseType)
        {
            foreach (var property in current.GetMembers().OfType<IPropertySymbol>())
            {
                if (property.SetMethod is null)
                    continue;

                if (!seen.Add(property))
                    continue;

                yield return property;
            }
        }
    }
}