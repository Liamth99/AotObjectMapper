using AutoMapper;
using BenchmarkDotNet.Attributes;
using MappingBenchmarks.Models;
using Microsoft.Extensions.Logging;

namespace MappingBenchmarks;

public partial class ComplexBenchmark
{
    private void ConfigureAutoMapper()
    {
        var loggerFactory = LoggerFactory.Create(_ => { });

        var config = new MapperConfiguration(cfg =>
                                             {
                                                 cfg.CreateMap<Person, PersonDto>()
                                                    .ForMember(d => d.FullName, expression => expression.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                                                    .ForMember(d => d.Street,   expression => expression.MapFrom(src => src.Address.Street))
                                                    .ForMember(d => d.City,     expression => expression.MapFrom(src => src.Address.City))
                                                    .ForMember(d => d.Country,  expression => expression.MapFrom(src => src.Address.Country))
                                                    .PreserveReferences();

                                                 cfg.CreateMap<Company, CompanyDto>()
                                                    .ForMember(d => d.FoundedAt,     expression => expression.MapFrom(src => src.Metadata.FoundedAt))
                                                    .ForMember(d => d.AnnualRevenue, expression => expression.MapFrom(src => src.Metadata.AnnualRevenue))
                                                    .PreserveReferences();

                                             }, loggerFactory);

        config.AssertConfigurationIsValid();
        _autoMapper = config.CreateMapper();
    }

    private IMapper _autoMapper = null!;

    [Benchmark, BenchmarkCategory("Reflection"), WarmupCount(10)]
    public PersonDto AutoMapper() => _autoMapper.Map<PersonDto>(_source);
}