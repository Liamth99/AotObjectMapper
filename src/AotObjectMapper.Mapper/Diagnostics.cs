using Microsoft.CodeAnalysis;

// ReSharper disable MemberCanBePrivate.Global

namespace AotObjectMapper.Mapper;

public static class Diagnostics
{
    public static class Categories
    {
        public const string StructureCategory  = "AOM.Structure";
        public const string AttributesCategory = "AOM.Attributes";
        public const string TypesCategory      = "AOM.Types";
        public const string SafetyCategory     = "AOM.Safety";
        public const string InternalCategory   = "AOM.Internal";
    }



    // AOM001 - AOM099
    // Structural Issues (fundamental breaks)

    public const string TypeMissingEmptyConstructorId = "AOM001";
    public static readonly DiagnosticDescriptor TypeMissingEmptyConstructor = new(
        id: TypeMissingEmptyConstructorId,
        title: "Type missing accessible parameterless constructor",
        messageFormat: "Type '{0}' must declare an accessible parameterless constructor",
        category: Categories.StructureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Maps need an empty constructor to create new objects with.",
        helpLinkUri: $"https://liamth99.github.io/AotObjectMapper/Analyzers/{TypeMissingEmptyConstructorId}");

    public const string MultipleDefaultFormatProviderId = "AOM002";
    public static readonly DiagnosticDescriptor MultipleDefaultFormatProvider = new(
        id: MultipleDefaultFormatProviderId,
        title: "Multiple Default Format Providers",
        messageFormat: "{0} has multiple default format providers",
        category: Categories.StructureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A mapper cannot have multiple default format providers.",
        helpLinkUri: $"https://liamth99.github.io/AotObjectMapper/Analyzers/{MultipleDefaultFormatProviderId}");


    // AOM101 - AOM199
    // Incorrect Attribute Usage



    // AOM201 - AOM299
    // Type & signature correctness



    // AOM301 - AOM399
    // Safety & best-practice warnings



    // AOM901 - AOM999
    // Internal Goofs
}