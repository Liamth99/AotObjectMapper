namespace AotObjectMapper.Abstractions.Enums;

/// <summary>
/// Defines options for specifying the accessibility levels of members and constructors
/// that the mapper is allowed to use when generating mappings.
/// </summary>
/// <remarks>
/// Enabling non-public access levels (<see cref="Internal"/>, <see cref="Protected"/>,
/// or <see cref="Private"/>) may cause the generated mapper to use
/// <c>UnsafeAccessor</c> based accessors to bypass normal accessibility restrictions.
/// </remarks>
[Flags]
public enum AccessibilityOptions
{
    ///
    None = 0,

    /// Allows public members and constructors to be included in operations.
    Public = 1,

    /// Allows internal members and constructors to be included in operations.
    Internal = 1 << 1,

    /// Allows protected members and constructors to be included in operations.
    Protected = 1 << 2,

    /// Allows private members and constructors to be included in operations.
    Private = 1 << 3,

    /// Includes <see cref="Internal"/>, <see cref="Protected"/> and <see cref="Private"/> members and constructors in operations.
    NonPublic = Internal | Protected | Private,

    /// Allows all members and constructors to be included in operations.
    All = Public | NonPublic,
}