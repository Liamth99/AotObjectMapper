namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Indicates that a specific format provider should be used for conversions
/// during mapping of properties. Useful when converting between types that
/// implement <see cref="IConvertible"/> to ensure proper formatting is applied.
/// </summary>
/// <typeparam name="TSource">The <see cref="IConvertible"/> type to convert from on the source entity type.</typeparam>
/// <typeparam name="TDestination">The target type to convert to on the target entity Type</typeparam>
/// <example>
/// <code>
/// class MapperClass
/// {
///     // Default
///     [UseFormatProvider]
///     private static IFormatProvider CurrentCulture => CultureInfo.CurrentCulture;
///     // int -> string only
///     [UseFormatProvider&lt;int, string&gt;]
///     private static IFormatProvider InvariantCulture => CultureInfo.InvariantCulture;
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class UseFormatProviderAttribute<TSource, TDestination> : Attribute where TSource : IConvertible;

/// <summary>
/// Specifies that a format provider should be used during the mapping of properties.
/// This attribute ensures proper formatting for type conversions where applicable.
/// </summary>
/// <example>
/// <code>
/// class MapperClass
/// {
///     // Default
///     [UseFormatProvider]
///     private static IFormatProvider CurrentCulture => CultureInfo.CurrentCulture;
///     // int -> string only
///     [UseFormatProvider&lt;int, string&gt;]
///     private static IFormatProvider InvariantCulture => CultureInfo.InvariantCulture;
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UseFormatProviderAttribute : Attribute;