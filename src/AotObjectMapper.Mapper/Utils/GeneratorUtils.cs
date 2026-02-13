using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace AotObjectMapper.Mapper.Utils;

public static class GeneratorUtils
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

    public static string EnumMapSwitchStatement(string sourceObjectName, ITypeSymbol source, ITypeSymbol destination, bool throwIfInvalid)
    {
        StringBuilder sb = new();

        sb.Append($"{sourceObjectName} switch {{ ");
        foreach (var fieldSymbol in source.GetMembers().OfType<IFieldSymbol>().Where(x => x.IsConst))
        {
            var destinationField = destination.GetMembers().OfType<IFieldSymbol>().SingleOrDefault(x => x.Name == fieldSymbol.Name);

            if (destinationField is not null)
                sb.Append($"global::{source.ToDisplayString()}.{fieldSymbol.Name} => global::{destination.ToDisplayString()}.{destinationField.Name}, ");
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
                    sb.Append($" global::{sourceType.ToDisplayString()} t{i} => Map(t{i}, ctx),");
                    i++;
                    break;
                }
            }

            foreach (var mapper in info.OtherMappers)
            {
                if (mapper.AttributeClass!.TypeArguments[1].Equals(sourceType, SymbolEqualityComparer.Default))
                {
                    sb.Append($" global::{sourceType.ToDisplayString()} t{i} => global::{mapper.AttributeClass!.TypeArguments[0].ToDisplayString()}.Map(t{i}, ctx),");
                    i++;
                    break;
                }
            }
        }

        sb.Append($"_ => throw new AotObjectMapper.Abstractions.Exceptions.UnhandledPolymorphicTypeException($\"Could not map type `{{{sourceObjectName}.GetType().FullName}}` to `{info.DestinationType.ToDisplayString()}` - no matching destination type found.\")");

        sb.Append(" }");

        return sb.ToString();
    }

    public static bool InstanceTypeMapSwitchStatement(string sourceObjectName, MethodGenerationInfo info, out string statement)
    {
        if (!info.PolymorphableTypes.TryGetValue(info.SourceType, out var sourceTypes) || !sourceTypes.Any())
        {
            statement = null!;
            return false;
        }

        StringBuilder sb = new();

        int i = 0;

        foreach (var sourceType in sourceTypes)
        {
            foreach (var map in info.Maps)
            {
                if (map.AttributeClass!.TypeArguments[0].Equals(sourceType, SymbolEqualityComparer.Default))
                {
                    sb.AppendLine($"if ({sourceObjectName} is global::{sourceType.ToDisplayString()} t{i}) return Map(t{i}, ctx);");
                    i++;
                    break;
                }
            }

            foreach (var mapper in info.OtherMappers)
            {
                if (mapper.AttributeClass!.TypeArguments[1].Equals(sourceType, SymbolEqualityComparer.Default))
                {
                    sb.AppendLine($"if ({sourceObjectName} is global::{sourceType.ToDisplayString()} t{i}) return global::{mapper.AttributeClass!.TypeArguments[0].ToDisplayString()}.Map(t{i}, ctx);");
                    i++;
                    break;
                }
            }
        }


        statement = sb.ToString();
        return true;
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
}