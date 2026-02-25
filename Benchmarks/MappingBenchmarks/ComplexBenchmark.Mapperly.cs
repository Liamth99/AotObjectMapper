using BenchmarkDotNet.Attributes;
using MappingBenchmarks.Models;
using Riok.Mapperly.Abstractions;

namespace MappingBenchmarks;

[Mapper(UseReferenceHandling = true)]
public partial class MapperlyComplexMapper
{
    [MapPropertyFromSource(nameof(PersonDto.FullName), Use = nameof(CombineFullName))]
    [MapProperty(nameof(@Person.Address.Street), nameof(PersonDto.Street))]
    [MapProperty(nameof(@Person.Address.City),   nameof(PersonDto.City))]
    [MapProperty(nameof(@Person.Address.Country), nameof(PersonDto.Country))]
    public partial PersonDto MapPerson(Person source);

    public string CombineFullName(Person source) => $"{source.FirstName} {source.LastName}";

    [MapProperty(nameof(@Company.Metadata.FoundedAt),     nameof(CompanyDto.FoundedAt))]
    [MapProperty(nameof(@Company.Metadata.AnnualRevenue), nameof(CompanyDto.AnnualRevenue))]
    public partial CompanyDto MapCompany(Company source);
}

public partial class ComplexBenchmark
{
    private MapperlyComplexMapper _mapperlyComplexMapper = null!;

    [Benchmark, BenchmarkCategory("Source Gen"), WarmupCount(10)]
    public PersonDto Mapperly()
    {
        var dto = _mapperlyComplexMapper.MapPerson(_source);
        _consumer.Consume(dto);
        return dto;
    }
}