namespace AotObjectMapper.Tests;

public partial class MapDepthTests
{
    [GenerateMapper(MappingOptions.PreserveReferences)]
    [Map<Parent, ParentDto>]
    [Map<Child, ChildDto>]
    public partial class DepthMapper
    {
        [PreMap<Parent, ParentDto>]
        private static void AssertDepthParent(Parent src, ParentDto dest, MapperContextBase ctx)
        {
            ctx.Depth.ShouldBe(1);
        }

        [PreMap<Child, ChildDto>]
        private static void AssertDepthChild(Child src, ChildDto dest, MapperContextBase ctx)
        {
            ctx.Depth.ShouldBe(2);
        }
    }

    [Fact]
    public void TestNormalBaseContext() => TestDepths(new MapperContext(maxDepth: 2));

    [Fact]
    public void TestConcurrentBaseContext() => TestDepths(new ConcurrentMapperContext(maxDepth: 2));

    private void TestDepths(MapperContextBase ctx)
    {
        Parent src = new() { Child = new Child() };
        src.Child.Parent = src;

        _ = DepthMapper.Map(src, ctx);
    }
}