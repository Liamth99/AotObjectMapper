using BenchmarkDotNet.Attributes;
using MappingBenchmarks.Models;

namespace MappingBenchmarks;

public partial class ComplexBenchmark
{
    public class ManualMapper
    {
        private Dictionary<object, object> _cache = [];

        public PersonDto MapPerson(Person source)
        {
            if (_cache.TryGetValue(source, out var existing))
                return (PersonDto)existing;

            var dto = new PersonDto
            {
                Id       = source.Id,
                FullName = $"{source.FirstName} {source.LastName}",
                Street   = source.Address.Street,
                City     = source.Address.City,
                Country  = source.Address.Country
            };

            _cache[source] = dto;

            dto.Employer = MapCompany(source.Employer);

            return dto;
        }

        public CompanyDto MapCompany(Company source)
        {
            if (_cache.TryGetValue(source, out var existing))
                return (CompanyDto)existing;

            var dto = new CompanyDto
            {
                Id            = source.Id,
                Name          = source.Name,
                FoundedAt     = source.Metadata.FoundedAt,
                AnnualRevenue = source.Metadata.AnnualRevenue
            };

            _cache[source] = dto;

            dto.CEO = MapPerson(source.CEO);

            dto.Employees = source.Employees.Select(MapPerson).ToArray();

            return dto;
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Baseline"), WarmupCount(10)]
    public PersonDto Manual()
    {
        var mapper = new ManualMapper();
        var dto = mapper.MapPerson(_source);

        _consumer.Consume(dto);
        return dto;
    }
}