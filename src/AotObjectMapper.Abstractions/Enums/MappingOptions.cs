using AotObjectMapper.Abstractions.Attributes;

namespace AotObjectMapper.Abstractions.Enums;

/// <summary>
/// Specifies options that can be used to control the behavior of object mapping.
/// </summary>
[Flags]
public enum MappingOptions : uint
{
    /// Default mapping behaviour.
    None = 0,

    /// Specifies that objects implementing <see cref="System.IConvertible"/> can be used during the mapping process.
    /// Use <see cref="UseFormatProviderAttribute{TSource,TDestination}">UseFormatProviderAttribute&lt;TSource,TDestination&gt;</see> or <see cref="UseFormatProviderAttribute"/> for formatting options.
    AllowIConvertable = 1,

    /// Indicates that null-related warnings should be suppressed during the mapping process.
    SuppressNullWarnings = 1 << 1,

    /// Specifies that object references should be preserved during the mapping process.
    PreserveReferences = 1 << 2,

    /// Indicates that enum-to-enum mappings should be performed based on their underlying numeric values. Defaults to mapping by name.
    MapEnumsByValue = 1 << 3,

    /// Throw exception if enum-to-enum mappings don't have a corresponding field name
    /// <remarks>Does nothing if <see cref="MapEnumsByValue"/> is set</remarks>
    ThrowExceptionOnUnmappedEnum = 1 << 4,
}