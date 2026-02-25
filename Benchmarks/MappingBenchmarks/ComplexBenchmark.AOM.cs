using AotObjectMapper.Abstractions.Attributes;
using AotObjectMapper.Abstractions.Enums;
using BenchmarkDotNet.Attributes;
using MappingBenchmarks.Models;

namespace MappingBenchmarks;

public partial class ComplexBenchmark
{
    [GenerateMapper(MappingOptions.PreserveReferences)]
    [Map<Person, PersonDto>]
    [Map<Company, CompanyDto>]
    public partial class AotObjectMapperPersonMapper
    {
        [ForMember<Person, PersonDto>(nameof(PersonDto.Street))]
        public static string GetStreet(Person source) => source.Address.Street;

        [ForMember<Person, PersonDto>(nameof(PersonDto.City))]
        public static string GetCity(Person source) => source.Address.City;

        [ForMember<Person, PersonDto>(nameof(PersonDto.Country))]
        public static string GetCountry(Person source) => source.Address.Country;

        [ForMember<Person, PersonDto>(nameof(PersonDto.FullName))]
        public static string GetFullName(Person source) => $"{source.FirstName} {source.LastName}";


        [ForMember<Company, CompanyDto>(nameof(CompanyDto.FoundedAt))]
        public static DateTime GetFoundedAt(Company source) => source.Metadata.FoundedAt;

        [ForMember<Company, CompanyDto>(nameof(CompanyDto.AnnualRevenue))]
        public static decimal GetAnnualRevenue(Company source) => source.Metadata.AnnualRevenue;
    }

    [Benchmark(Description = "> AotObjectMapper"), BenchmarkCategory("Source Gen"), WarmupCount(10)]
    public PersonDto AotObjectMapper()
    {
        var dto = AotObjectMapperPersonMapper.Map(_source);
        _consumer.Consume(dto);
        return dto;
    }
}