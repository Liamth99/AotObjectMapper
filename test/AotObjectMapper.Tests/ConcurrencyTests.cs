using System.Collections.Concurrent;

namespace AotObjectMapper.Tests;

public partial class ConcurrencyTests
{
    public class Entity
    {
        public Guid      Id     { get; set; }
        public int       Number { get; set; }
        public SubEntity Sub    { get; set; } = null!;
    }

    public class SubEntity
    {
        public string Text => "Hello";
    }

    public class Dto
    {
        public Guid   Id     { get; set; }
        public int    Number { get; set; }
        public SubDto Sub    { get; set; } = null!;
    }

    public class SubDto
    {
        public string Text { get; set; } = string.Empty;
    }

    [GenerateMapper(MappingOptions.PreserveReferences)]
    [Map<Entity, Dto>]
    [Map<SubEntity, SubDto>]
    public partial class ConcurrentUserMapper
    {
        [PreMap<Entity, Dto>]
        private static void AddToContext(Entity src, Dto _, MapperContextBase ctx)
        {
            Thread.Yield(); // increase chance of race
            ctx.Depth.ShouldBe(1);
            ctx.AdditionalContext.TryAdd(src.Id.ToString(), src.Number).ShouldBeTrue();
            Interlocked.Increment(ref _adds);
        }

        [PreMap<SubEntity, SubDto>]
        private static void AssertChildDepth(SubEntity src, SubDto dest, MapperContextBase ctx) => ctx.Depth.ShouldBe(2);

        [PostMap<Entity, Dto>]
        private static void RemoveFromContext(Dto dto, MapperContextBase ctx)
        {
            Thread.Yield(); // increase chance of race
            ctx.AdditionalContext.Remove(dto.Id.ToString(), out var number).ShouldBeTrue();
            ctx.Depth.ShouldBe(1);
            number.ShouldBe(dto.Number);
            Interlocked.Increment(ref _removes);
        }
    }

    private static int _adds;
    private static int _removes;

    private const int RunCount = 100_000;

    [Fact]
    public async Task ContextIsThreadSafe_UnderParallelMapping_NewContext()
    {
        _adds = 0;
        _removes = 0;

        ConcurrentDictionary<string, object> data = new();
        await Parallel.ForAsync(0,
                                RunCount,
                                (i, _) =>
                                {
                                    var ctx = new ConcurrentMapperContext(data);
                                    var src = new Entity() { Id = Guid.NewGuid(), Number = i, Sub = new SubEntity(), };
                                    var dto = ConcurrentUserMapper.Map(src, ctx);

                                    return ValueTask.CompletedTask;
                                });

        data.ShouldBeEmpty();
        _adds.ShouldBe(RunCount);
        _removes.ShouldBe(RunCount);
    }

    [Fact]
    public async Task ContextIsThreadSafe_UnderParallelMapping_ExistingContext()
    {
        _adds    = 0;
        _removes = 0;

        var ctx = new ConcurrentMapperContext();

        await Parallel.ForAsync(0,
                                RunCount,
                                (i, _) =>
                                {
                                    var src = new Entity() { Id = Guid.NewGuid(), Number = i, Sub = new SubEntity(), };
                                    var dto = ConcurrentUserMapper.Map(src, ctx);

                                    return ValueTask.CompletedTask;
                                });

        ctx.AdditionalContext.ShouldBeEmpty();
        _adds.ShouldBe(RunCount);
        _removes.ShouldBe(RunCount);
    }
}