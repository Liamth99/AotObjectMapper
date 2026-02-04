using System.Drawing;
using AotObjectMapper.Abstractions.Attributes;
using AotObjectMapper.Abstractions.Models;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MappingBenchmarks.Models;
using Mapster;
using Microsoft.Extensions.Logging;

namespace MappingBenchmarks;

[AotObjectMapper.Abstractions.Attributes.GenerateMapper]
[Map<SimpleStruct, SimpleStructDto>]
public partial class AotObjectMapperSimpleStructMapper;

[Riok.Mapperly.Abstractions.Mapper]
public partial class MapperlySimpleStructMapper
{
    public partial SimpleStructDto SimpleToDto(SimpleStruct simple);
}

[MemoryDiagnoser(displayGenColumns: false)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[CategoriesColumn]
public class SimpleStructBenchmark
{
    private SimpleStruct _source;

    [GlobalSetup]
    public void Setup()
    {
        _source = new SimpleStruct
        {
            Id                = 1,
            Name              = "Test",
            Age               = 1_000,
            CreatedAt         = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Amount            = 123.45m,
            FavouriteColor    = Color.Aqua,
            FavouriteIceCream = "Strawberry",
        };

        var loggerFactory = LoggerFactory.Create(_ => { });

        var config = new MapperConfiguration(cfg =>
                                             {
                                                 cfg.CreateMap<SimpleStruct, SimpleStructDto>();
                                             }, loggerFactory);

        _autoMapper           = config.CreateMapper();

        _mapperlySimpleStructMapper = new MapperlySimpleStructMapper();
        _ctx                  = new MapperContext();

        _ = _source.Adapt<SimpleStructDto>();          // warm Mapster
        _ = _autoMapper.Map<SimpleStructDto>(_source); // warm AutoMapper
    }

    // -----------------
    // AotObjectMapper
    // -----------------

    private MapperContext _ctx = null!;

    [Benchmark(Description = "> AotObjectMapper"), BenchmarkCategory("Source Gen"), WarmupCount(10)]
    public SimpleStructDto AotObjectMapper() => AotObjectMapperSimpleStructMapper.Map(_source, _ctx);

    // -----------------
    // Mapster
    // -----------------

    [Benchmark, BenchmarkCategory("Reflection"), WarmupCount(10)]
    public SimpleStructDto Mapster() => _source.Adapt<SimpleStructDto>();

    // -----------------
    // AutoMapper
    // -----------------

    private IMapper _autoMapper = null!;

    [Benchmark, BenchmarkCategory("Reflection"), WarmupCount(10)]
    public SimpleStructDto AutoMapper() => _autoMapper.Map<SimpleStructDto>(_source);

    // -----------------
    // Maperly
    // -----------------

    private MapperlySimpleStructMapper _mapperlySimpleStructMapper = null!;

    [Benchmark, BenchmarkCategory("Source Gen"), WarmupCount(10)]
    public SimpleStructDto Mapperly() => _mapperlySimpleStructMapper.SimpleToDto(_source);

    // -----------------
    // Manual mapping (baseline)
    // -----------------

    [Benchmark(Baseline = true), BenchmarkCategory("Baseline"), WarmupCount(10)]
    public SimpleStructDto Manual()
    {
        return new SimpleStructDto
        {
            Id                = _source.Id,
            Name              = _source.Name,
            Age               = _source.Age,
            CreatedAt         = _source.CreatedAt,
            Amount            = _source.Amount,
            FavouriteColor    = _source.FavouriteColor,
            FavouriteIceCream = _source.FavouriteIceCream,
        };
    }
}