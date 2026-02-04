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
[Map<Simple, SimpleDto>]
public partial class AotObjectMapperSimpleMapper;

[Riok.Mapperly.Abstractions.Mapper]
public partial class MapperlySimpleMapper
{
    public partial SimpleDto SimpleToDto(Simple simple);
}

[MemoryDiagnoser(displayGenColumns: false)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[CategoriesColumn]
public class SimpleBenchmark
{
    private Simple _source = null!;

    [GlobalSetup]
    public void Setup()
    {
        _source = new Simple
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
                                                 cfg.CreateMap<Simple, SimpleDto>();
                                             }, loggerFactory);

        _autoMapper           = config.CreateMapper();

        _mapperlySimpleMapper = new MapperlySimpleMapper();
        _ctx                  = new MapperContext();

        _ = _source.Adapt<SimpleDto>();          // warm Mapster
        _ = _autoMapper.Map<SimpleDto>(_source); // warm AutoMapper
    }

    // -----------------
    // AotObjectMapper
    // -----------------

    private MapperContext _ctx = null!;

    [Benchmark(Description = "> AotObjectMapper"), BenchmarkCategory("Source Gen")]
    public SimpleDto AotObjectMapper() => AotObjectMapperSimpleMapper.Map(_source, _ctx);

    // -----------------
    // Mapster
    // -----------------

    [Benchmark, BenchmarkCategory("Reflection")]
    public SimpleDto Mapster() => _source.Adapt<SimpleDto>();

    // -----------------
    // AutoMapper
    // -----------------

    private IMapper _autoMapper = null!;

    [Benchmark, BenchmarkCategory("Reflection")]
    public SimpleDto AutoMapper() => _autoMapper.Map<SimpleDto>(_source);

    // -----------------
    // Maperly
    // -----------------

    private MapperlySimpleMapper _mapperlySimpleMapper = null!;

    [Benchmark, BenchmarkCategory("Source Gen")]
    public SimpleDto Mapperly() => _mapperlySimpleMapper.SimpleToDto(_source);

    // -----------------
    // Manual mapping (baseline)
    // -----------------

    [Benchmark(Baseline = true), BenchmarkCategory("Baseline")]
    public SimpleDto Manual()
    {
        return new SimpleDto
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