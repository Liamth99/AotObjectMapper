namespace AotObjectMapper.Abstractions.Attributes;

/// <summary>
/// Attribute that specifies a mapping behavior from a source type to a destination type for a specific member.
/// This is used in the context of mapping generation to associate a method with a particular member transformation.
/// </summary>
/// <typeparam name="TSource">The source type from which the mapping originates.</typeparam>
/// <typeparam name="TDestination">The destination type to which the mapping is applied.</typeparam>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ForMemberAttribute<TSource, TDestination> : Attribute
{
    public ForMemberAttribute(string mapMethodName, string memberName)
    {
        MapMethodName = mapMethodName;
        MemberName    = memberName;
    }

    public ForMemberAttribute(string memberName)
    {
        MapMethodName = MapAttribute<TSource, TDestination>.DefaultMapMethodName;
        MemberName    = memberName;
    }

    public string MapMethodName { get; }
    public string MemberName    { get; }
}