using BenchmarkDotNet.Attributes;
using MappingBenchmarks.Models;
using Mapster;

namespace MappingBenchmarks;

public partial class ComplexBenchmark
{
    private void ConfigureMapster()
    {
        _mapsterConfig = new TypeAdapterConfig();

        _mapsterConfig.NewConfig<Person, PersonDto>()
                      .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                      .Map(dest => dest.Street,   src => src.Address.Street)
                      .Map(dest => dest.City,     src => src.Address.City)
                      .Map(dest => dest.Country,  src => src.Address.Country)
                      .PreserveReference(true);

        _mapsterConfig.NewConfig<Company, CompanyDto>()
                      .Map(dest => dest.AnnualRevenue, src => src.Metadata.AnnualRevenue)
                      .Map(dest => dest.FoundedAt,   src => src.Metadata.FoundedAt)
                      .PreserveReference(true);
    }

    private TypeAdapterConfig _mapsterConfig = null!;

    [Benchmark, BenchmarkCategory("Reflection"), WarmupCount(10)]
    public PersonDto Mapster()
    {
        var dto = _source.Adapt<PersonDto>(_mapsterConfig);
        _consumer.Consume(dto);
        return dto;
    }
}