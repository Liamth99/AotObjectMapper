using System.Collections.Generic;
using System.Linq;
using AotObjectMapper.Mapper.Info;
using Microsoft.CodeAnalysis;

namespace AotObjectMapper.Mapper.Extensions;

public static class ITypeSymbolExtensions
{
    extension(ITypeSymbol type)
    {
        public IEnumerable<IPropertySymbol> GetAllReadableProperties()
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

        public IEnumerable<IPropertySymbol> GetAllSetableProperties()
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

        public bool IsSimpleType()
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

        public bool TryGetBlankTypeConstructor(MethodGenerationInfo? info, out string ctorCode, out (string type, string argName)[] arguments)
        {
            if (type is not INamedTypeSymbol namedType)
            {
                arguments = [];
                ctorCode  = string.Empty;
                return false;
            }

            if (info?.FactoryMethod is not null)
            {
                if (info.FactoryMethod?.Parameters.Length is 1)
                {
                    arguments = [new("global::AotObjectMapper.Abstractions.Models.MapperContext" ,"ctx")];
                    ctorCode  = $"{info.FactoryMethod.Name}(ctx)";
                    return true;
                }
                else
                {
                    arguments = [];
                    ctorCode  = $"{info.FactoryMethod.Name}()";
                    return true;
                }

            }

            arguments = [];

            if (namedType.Constructors.Any(x => x.DeclaredAccessibility is Accessibility.Public && x.Parameters.Length is 0))
            {
                ctorCode = $"new global::{type.ToDisplayString()}() {{ {string.Join(" ", type.GetMembers().OfType<IPropertySymbol>().Where(p => p.SetMethod is not null && p.IsRequired).Select(x => $"{x.Name} = {(x.Type.IsReferenceType ? "null!" : "default")},"))} }}";
                return true;
            }

            ctorCode = string.Empty;
            return false;
        }
    }
}