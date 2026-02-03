using System.ComponentModel;

namespace AotObjectMapper.Tests;


[GenerateMapper(MappingOptions.PreserveReferences)]
[Map<Parent, ParentDto>]
[UseMap<ChildMapper, Child, ChildDto>]
public partial class ParentMapper;

[GenerateMapper(MappingOptions.PreserveReferences)]
[Map<Child, ChildDto>]
[UseMap<ParentMapper, Parent, ParentDto>]
public partial class ChildMapper;

public class ReferencePreservationTests
{
    [Fact]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void CircularReference_NoInfiniteRecursion()
    {
        var parent = new Parent();
        var child = new Child(){ Parent = parent };

        parent.Child = child;

        var parentDto = ParentMapper.Map(parent);

        ReferenceEquals(parentDto.Child.Parent, parentDto).ShouldBeTrue();
    }
}