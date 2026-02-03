using AotObjectMapper.Abstractions.Enums;

namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// An attribute that provides metadata to generate a mapper for the annotated class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GenerateMapperAttribute(MappingOptions options = MappingOptions.None) : Attribute
{
    public MappingOptions MappingOptions { get; } = options;
}