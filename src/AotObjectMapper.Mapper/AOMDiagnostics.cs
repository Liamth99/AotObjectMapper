using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace AotObjectMapper.Mapper;

public static class AOMDiagnostics
{

    public static ImmutableArray<DiagnosticDescriptor> DiagnosticDescriptors()
    {
        return [..typeof(AOMDiagnostics)
                 .GetFields(BindingFlags.Static | BindingFlags.Public)
                 .Where(x => x.FieldType == typeof(DiagnosticDescriptor))
                 .Select(x => (DiagnosticDescriptor)x.GetValue(null))];
    }

    public static class DiagnosticCategories
    {
        public const string Internal      = "Internal";
        public const string Usage         = "Usage";
        public const string Configuration = "Configuration";
        public const string TypeSafety    = "TypeSafety";
        public const string Design        = "Design";
        public const string Performance   = "Performance";
    }
    
    // *****************
    // Diagnostic Codes:
    // *****************
    
    // Internal Error Diagnostics
    // 000 - 099
    public const string UnhandledExceptionId = "AOM000";
    public static readonly DiagnosticDescriptor AOM000_UnhandledExceptionId = new (
        id: UnhandledExceptionId, 
        title: "Unhandled exception while generating mapper",
        messageFormat: "Unhandled exception while generating `{0}`. {1}.",
        category: DiagnosticCategories.Internal, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true, 
        description: "Unhandled exception while generating mapper please submit issue on github.",
        helpLinkUri: "https://github.com/Liamth99/AotObjectMapper/issues", 
        customTags: [WellKnownDiagnosticTags.AnalyzerException, WellKnownDiagnosticTags.NotConfigurable]); 
    
    
    // Incorrect Attribute Usage
    // 100 - 199
    public const string MethodHasIncorrectSignatureReturnTypeId = "AOM100";
    public static readonly DiagnosticDescriptor AOM100_MethodHasIncorrectSignatureReturnType = new (
        id: MethodHasIncorrectSignatureReturnTypeId, 
        title: "Method has incorrect return type",
        messageFormat: "`{0}` should return type `{1}`",
        category: DiagnosticCategories.Usage, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 
    
    public const string MethodHasIncorrectSignatureSourceParameterId = "AOM101";
    public static readonly DiagnosticDescriptor AOM101_MethodHasIncorrectSignatureSourceParameter = new (
        id: MethodHasIncorrectSignatureSourceParameterId, 
        title: "Method has incorrect source parameter type",
        messageFormat: "First parameter for`{0}` should be of type `{1}`",
        category: DiagnosticCategories.Usage, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 
    
    public const string MethodHasIncorrectSignatureMapperContextId = "AOM102";
    public static readonly DiagnosticDescriptor AOM102_MethodHasIncorrectSignatureMapperContext = new (
        id: MethodHasIncorrectSignatureMapperContextId, 
        title: "Method has incorrect MapperContext parameter type",
        messageFormat: "Second parameter for `{0}` should either be removed or be of type `MapperContextBase`",
        category: DiagnosticCategories.Usage, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 
    
    public const string MethodDoesNotRequireContextId = "AOM103";
    public static readonly DiagnosticDescriptor AOM103_MethodDoesNotRequireContext = new (
        id: MethodDoesNotRequireContextId, 
        title: "Method does not require MapperContextBase and the parameter should be removed",
        messageFormat: "`{0}` does not require MapperContextBase. Remove the parameter to avoid unnecessary overhead.",
        category: DiagnosticCategories.Performance, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true);

    public const string MethodHasIncorrectSignatureNotStaticId = "AOM104";
    public static readonly DiagnosticDescriptor AOM104_MethodHasIncorrectSignatureNotStatic = new (
        id: MethodHasIncorrectSignatureNotStaticId,
        title: "Method must be static",
        messageFormat: "`{0}` must be static",
        category: DiagnosticCategories.Usage,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    
    // Invalid Mapper Configuration
    // 200 - 299
    public const string MapsMustBeDistinctId = "AOM200";
    public static readonly DiagnosticDescriptor AOM200_MapsMustBeDistinct = new (
        id: MapsMustBeDistinctId, 
        title: "Maps must be distinct",
        messageFormat: "Map for {0} -> {1} already configured via `{2}`",
        category: DiagnosticCategories.Configuration, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true, 
        description: "Mappers cannot have any ambiguity between type pairs when mapping."); 

    public const string MemberNamesShouldBeValidId = "AOM201";
    public static readonly DiagnosticDescriptor AOM201_MemberNamesShouldBeValid = new (
        id: MemberNamesShouldBeValidId, 
        title: "Member names should be valid",
        messageFormat: "`{0}` is not a valid member name for type `{1}`",
        category: DiagnosticCategories.Configuration, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 

    public const string PreferNameOfId = "AOM202";
    public static readonly DiagnosticDescriptor AOM202_PreferNameOf = new (
        id: PreferNameOfId, 
        title: "Prefer using `nameof()` over raw string",
        messageFormat: "`{0}` is a valid member name, but nameof({1}) is preferred",
        category: DiagnosticCategories.Design, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true); 

    public const string UseFormatProviderDestinationShouldBeValidTypeId = "AOM203";
    public static readonly DiagnosticDescriptor AOM203_UseFormatProviderDestinationShouldBeValidType = new (
        id: UseFormatProviderDestinationShouldBeValidTypeId, 
        title: "UseFormatProvider destination type should be valid type",
        messageFormat: "{0} is not a valid destination type for a format provider",
        category: DiagnosticCategories.Usage, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true,
        helpLinkUri: "https://learn.microsoft.com/en-us/dotnet/api/system.iconvertible?view=netstandard-2.0"); 

    public const string PotentialRecursiveMappingId = "AOM204";
    public static readonly DiagnosticDescriptor AOM204_PotentialRecursiveMapping = new (
        id: PotentialRecursiveMappingId, 
        title: "Potential recursive mapping detected",
        messageFormat: "Potential recursive mapping detected, add MappingOptions.PreserveReferences to options or use mapping depth limits",
        category: DiagnosticCategories.Design, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true);

    public const string MapperShouldOnlyHaveOneDefaultFormatProviderId = "AOM205";
    public static readonly DiagnosticDescriptor AOM205_MapperShouldOnlyHaveOneDefaultFormatProvider = new (
        id: MapperShouldOnlyHaveOneDefaultFormatProviderId, 
        title: "Mapper should only have one default FormatProvider",
        messageFormat: "Mapper should only have one default FormatProvider",
        category: DiagnosticCategories.Usage, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 

    public const string MapperShouldOnlyHaveOneFormatProviderForTypePairId = "AOM206";
    public static readonly DiagnosticDescriptor AOM206_MapperShouldOnlyHaveOneFormatProviderForTypePair = new (
        id: MapperShouldOnlyHaveOneFormatProviderForTypePairId, 
        title: "Mapper should only have one FormatProvider for type pair",
        messageFormat: "Mapper should only have one FormatProvider pair",
        category: DiagnosticCategories.Usage, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 

    public const string NoConstructorId = "AOM207";
    public static readonly DiagnosticDescriptor AOM207_NoConstructor = new (
        id: NoConstructorId, 
        title: "No accessible constructor or factory method",
        messageFormat: "No accessible constructor or factory method found for {0}",
        category: DiagnosticCategories.Configuration, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 

    public const string IgnoredMemberDoesNotExistId = "AOM208";
    public static readonly DiagnosticDescriptor AOM208_IgnoredMemberDoesNotExist = new (
        id: IgnoredMemberDoesNotExistId, 
        title: "Ignored member does not exist",
        messageFormat: "Ignored member `{0}` does not exist on `{1}`",
        category: DiagnosticCategories.Configuration, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true); 

    public const string DuplicateConfigurationForMemberId = "AOM209";
    public static readonly DiagnosticDescriptor AOM209_DuplicateConfigurationForMember = new (
        id: DuplicateConfigurationForMemberId, 
        title: "Duplicate configuration for member",
        messageFormat: "Configuration for `{0}` already exists",
        category: DiagnosticCategories.Configuration, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 

    
    // Type Compatibility / Conversion Issues
    // 300 - 399
    public const string NoMapFoundForDestinationTypeId = "AOM300";
    public static readonly DiagnosticDescriptor AOM300_NoMapFoundForDestinationType = new (
        id: NoMapFoundForDestinationTypeId, 
        title: "No map found for destination type",
        messageFormat: "No map found for `{0}` -> `{1}` member {2} will be ignored, either reconfigure mapper or use another correctly configured mapper",
        category: DiagnosticCategories.TypeSafety, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true);

    public const string NullableAssignmentToNonNullablePropertyId = "AOM301";
    public static readonly DiagnosticDescriptor AOM301_NullableAssignmentToNonNullableProperty = new (
        id: NullableAssignmentToNonNullablePropertyId, 
        title: "Nullable assignment to non-nullable property",
        messageFormat: "`{0}` has a nullable source with a non-nullable destination",
        category: DiagnosticCategories.TypeSafety, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true);

    public const string RequiredMemberNotMappedId = "AOM302";
    public static readonly DiagnosticDescriptor AOM302_RequiredMemberNotMapped = new (
        id: RequiredMemberNotMappedId, 
        title: "Required member not mapped",
        messageFormat: "Required member `{0}` on `{1}` should be mapped",
        category: DiagnosticCategories.TypeSafety, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true);

    
    // Missing Or Unmapped members
    // 400 - 499
    public const string UnmappedDestinationMemberId = "AOM400";
    public static readonly DiagnosticDescriptor AOM400_UnmappedDestinationMember = new (
        id: UnmappedDestinationMemberId, 
        title: "Unmapped destination member",
        messageFormat: "`{0}` on `{1}` has not been mapped",
        category: DiagnosticCategories.Configuration, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true);

    public const string PropertyNotPubliclySettableId = "AOM401";
    public static readonly DiagnosticDescriptor AOM401_PropertyNotPubliclySettable = new (
        id: PropertyNotPubliclySettableId, 
        title: "Property not publicly settable",
        messageFormat: "`{0}` is not publicly settable",
        category: DiagnosticCategories.Design, 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true); 

    
    // Potential Data-loss / Ambiguity
    // 500 - 599
    public const string IConvertibleMayResultInDataLossId = "AOM500";
    public static readonly DiagnosticDescriptor AOM500_IConvertibleMayResultInDataLoss = new (
        id: IConvertibleMayResultInDataLossId, 
        title: "Potential loss of data when converting between types",
        messageFormat: "Converting from {0} to {1} may result in loss of data",
        category: DiagnosticCategories.TypeSafety, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true); 

    public const string DestinationEnumMissingValueId = "AOM501";
    public static readonly DiagnosticDescriptor AOM501_DestinationEnumMissingValue = new (
        id: DestinationEnumMissingValueId, 
        title: "Enum value does not exist on destination",
        messageFormat: "Field `{0}` does not exist on {1} and will be mapped to default",
        category: DiagnosticCategories.TypeSafety, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true); 

    public const string DestinationEnumMissingValueExceptionId = "AOM502";
    public static readonly DiagnosticDescriptor AOM502_DestinationEnumMissingValueException = new (
        id: DestinationEnumMissingValueExceptionId, 
        title: "Enum value does not exist on destination",
        messageFormat: "Field `{0}` does not exist on {1} and will throw an exception",
        category: DiagnosticCategories.TypeSafety, 
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true); 

}