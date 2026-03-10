using AotObjectMapper.Abstractions.Enums;

namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// An attribute that provides metadata to generate a mapper for the annotated class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GenerateMapperAttribute(MappingOptions options = MappingOptions.None, AccessibilityOptions memberAccessibility = AccessibilityOptions.Public) : Attribute
{
    /// Specifies options that can be used to modify the behavior of object mapping operations.
    public MappingOptions MappingOptions { get; } = options;

    /// Defines the accessibility levels for members that can be included during mapping operations.
    public AccessibilityOptions MemberAccessibility { get; } = memberAccessibility;
}