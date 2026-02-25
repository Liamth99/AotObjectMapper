using Riok.Mapperly.Abstractions;

namespace MappingBenchmarks.Models;

// Entities

public class Person
{
    public int Id { get; set; }

    [MapperIgnore]
    public string FirstName { get; set; } = string.Empty;
    [MapperIgnore]
    public string LastName  { get; set; } = string.Empty;

    // Nested object
    public Address Address { get; set; } = null!;

    // Circular reference
    public Company Employer { get; set; } = null!;
}

public class Address
{
    public string Street  { get; set; } = string.Empty;
    public string City    { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class Company
{
    public int    Id   { get; set; }
    public string Name { get; set; } = string.Empty;

    // Nested object
    public CompanyMetadata Metadata { get; set; } = null!;

    // Circular reference back to Person
    public Person CEO { get; set; } = null!;

    public IEnumerable<Person> Employees { get; set; } = null!;
}

public class CompanyMetadata
{
    public DateTime FoundedAt     { get; set; }
    public decimal  AnnualRevenue { get; set; }
}

// DTOs

public class PersonDto
{
    public int Id { get; set; }

    // Custom mapping: FirstName + LastName
    public string FullName { get; set; } = string.Empty;

    // Flattened Address
    public string Street  { get; set; } = string.Empty;
    public string City    { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public CompanyDto Employer { get; set; } = null!;
}

public class CompanyDto
{
    public int    Id   { get; set; }
    public string Name { get; set; } = string.Empty;

    // Flattened metadata
    public DateTime FoundedAt     { get; set; }
    public decimal  AnnualRevenue { get; set; }

    public PersonDto CEO { get; set; } = null!;

    public PersonDto[] Employees { get; set; } = null!;
}