using AotObjectMapper.Abstractions.Attributes;
using AotObjectMapper.Abstractions.Enums;
using BenchmarkDotNet.Attributes;
using MappingBenchmarks.Models;

namespace MappingBenchmarks;

[GenerateMapper(MappingOptions.PreserveReferences)]
[Map<Person, PersonDto>]
[UseMap<AotObjectMapperCompanyMapper, Company, CompanyDto>]
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
}


[GenerateMapper(MappingOptions.PreserveReferences)]
[Map<Company, CompanyDto>]
[UseMap<AotObjectMapperPersonMapper, Person, PersonDto>]
public partial class AotObjectMapperCompanyMapper
{
    [ForMember<Company, CompanyDto>(nameof(CompanyDto.FoundedAt))]
    public static DateTime GetFoundedAt(Company source) => source.Metadata.FoundedAt;

    [ForMember<Company, CompanyDto>(nameof(CompanyDto.AnnualRevenue))]
    public static decimal GetFAnnualRevenue(Company source) => source.Metadata.AnnualRevenue;
}

public partial class ComplexBenchmark
{
    [Benchmark(Description = "> AotObjectMapper"), BenchmarkCategory("Source Gen"), WarmupCount(10)]
    public PersonDto AotObjectMapper() => AotObjectMapperPersonMapper.Map(_source);

}