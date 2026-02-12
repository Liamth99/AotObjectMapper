namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Marks a method as a factory for creating instances of the specified destination type.
/// </summary>
/// <typeparam name="TDestination">The type of object that the factory method produces.</typeparam>
[AttributeUsage(AttributeTargets.Method)]
public class UseFactoryAttribute<TDestination> : Attribute;