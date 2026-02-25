using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using MappingBenchmarks.Models;

namespace MappingBenchmarks;

[MemoryDiagnoser(displayGenColumns: false)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[CategoriesColumn]
public partial class ComplexBenchmark
{
    private Person   _source  = null!;
    private Consumer _consumer = new();

    [GlobalSetup]
    public void Setup()
    {
        _source = new Person()
        {
            Id = 1,
            FirstName = "Greg",
            LastName = "Smith",
            Address = new Address()
            {
                Street = "Elm Street",
                City = "Somewhere",
                Country = "America",
            },
            Employer = null!,
        };

        var company = new Company()
        {
            Id   = 5,
            Name = "Company inc.",
            Metadata =
                new CompanyMetadata()
                {
                    AnnualRevenue = 5,
                    FoundedAt     = new DateTime(2026, 1, 1, 0, 0, 0),
                },
            CEO = _source,
        };

        var secondAddress = new Address() { Street = "Main street", City = "Main city", Country = "Italy?" };

        company.CEO      = _source;
        _source.Employer = company;
        company.Employees =
        [
            _source,
            new Person { Id = 2, FirstName = "1", LastName = "1", Address = secondAddress, Employer  = company, },
            new Person { Id = 3, FirstName = "2", LastName = "2", Address = secondAddress, Employer  = company, },
            new Person { Id = 4, FirstName = "3", LastName = "3", Address = secondAddress, Employer  = company, },
            new Person { Id = 5, FirstName = "4", LastName = "4", Address = secondAddress, Employer  = company, },
        ];

        _mapperlyComplexMapper = new MapperlyComplexMapper();
        ConfigureAutoMapper();
        ConfigureMapster();

        // Verify all outputs are equal

        var jsonOpts = new JsonSerializerOptions() { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };

        Console.WriteLine("Verifing mappers...");

        var manual   = JsonSerializer.Serialize(Manual(), jsonOpts);
#if Debug
        Console.WriteLine("Manual:");
        Console.WriteLine(manual);
#endif

        var aot      = JsonSerializer.Serialize(AotObjectMapper(), jsonOpts);
#if Debug
        Console.WriteLine("aot:");
        Console.WriteLine(aot);
#endif

        var mapperly = JsonSerializer.Serialize(Mapperly(), jsonOpts);
#if Debug
        Console.WriteLine("mapperly:");
        Console.WriteLine(mapperly);
#endif

        var auto     = JsonSerializer.Serialize(AutoMapper(), jsonOpts);
#if Debug
        Console.WriteLine("auto:");
        Console.WriteLine(auto);
#endif

        var mapster  = JsonSerializer.Serialize(Mapster(), jsonOpts);
#if Debug
        Console.WriteLine("mapster:");
        Console.WriteLine(mapster);
#endif

#pragma warning disable LOCAT010
        if (aot != manual)
            throw new Exception("AotObjectMapper and manual maps are not equal.");

        if (mapperly != manual)
            throw new Exception("Mapperly and manual maps are not equal.");

        if (auto != manual)
            throw new Exception("Automapper and manual maps are not equal.");

        if (mapster != manual)
            throw new Exception("Mapster and manual maps are not equal.");
#pragma warning restore LOCAT010
    }
}