namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Indicates that the parameter should prefer the use of the <c>nameof()</c> operator for referring to identifiers.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class PreferNameOfAttribute(string targetType) : Attribute
{
    /// Gets the type preferred for usage. This property can be used to indicate the target type in scenarios where <c>nameof()</c> is being employed.
    public string TargetType { get; } = targetType;
}