using BenchmarkDotNet.Attributes;
using MappingBenchmarks.Models;

namespace MappingBenchmarks;

public partial class ComplexBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("Baseline"), WarmupCount(10)]
    public PersonDto Manual()
    {
        var dto = new PersonDto
        {
            Id       = _source.Id,
            FullName = $"{_source.FirstName} {_source.LastName}",
            Street   = _source.Address.Street,
            City     = _source.Address.City,
            Country  = _source.Address.Country,
            Employer = new CompanyDto()
            {
                Id            = _source.Employer.Id,
                Name          = _source.Employer.Name,
                AnnualRevenue = _source.Employer.Metadata.AnnualRevenue,
                FoundedAt     = _source.Employer.Metadata.FoundedAt,
                CEO           = null!,
            },
        };

        dto.Employer.CEO = dto;

        _consumer.Consume(dto);
        return dto;
    }
}