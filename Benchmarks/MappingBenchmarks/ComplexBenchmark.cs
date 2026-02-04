using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using MappingBenchmarks.Models;

namespace MappingBenchmarks;

[MemoryDiagnoser(displayGenColumns: true)]
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
                Resident = null!,
            },
            Employer = new Company()
            {
                Id = 5,
                Name = "Company inc.",
                Metadata = new CompanyMetadata()
                {
                    AnnualRevenue = 5,
                    FoundedAt = new DateTime(2026, 1, 1, 0, 0, 0),
                },
                CEO = null!,
            },
        };

        _source.Address.Resident = _source;
        _source.Employer.CEO = _source;

        _mapperlyComplexMapper = new MapperlyComplexMapper();
        ConfigureAutoMapper();
        ConfigureMapster();

        // Verify all outputs are equal

        var jsonOpts = new JsonSerializerOptions() { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };

        var manual   = JsonSerializer.Serialize(Manual(),          jsonOpts);

        var aot      = JsonSerializer.Serialize(AotObjectMapper(), jsonOpts);
        var mapperly = JsonSerializer.Serialize(Mapperly(),        jsonOpts);
        var auto     = JsonSerializer.Serialize(AutoMapper(),      jsonOpts);
        var mapster     = JsonSerializer.Serialize(Mapster(),      jsonOpts);

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