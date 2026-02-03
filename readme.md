# AotObjectMapper
![NuGet Version](https://img.shields.io/nuget/v/AotObjectMapper.svg)
![NuGet Downloads](https://img.shields.io/nuget/dt/AotObjectMapper.svg)
![GitHub Release](https://img.shields.io/github/v/release/Liamth99/AotObjectMapper)
![License](https://img.shields.io/github/license/Liamth99/AotObjectMapper)

AotObjectMapper is a C# Roslyn source generator for compile-time object mapping.

<p align="center">
<img src="/banner1280x640.png" alt="Simple Icons" height=256>
</p>

# Installation

AotObjectMapper is distributed as a [NuGet package](https://www.nuget.org/packages/AotObjectMapper/).

## Via .NET CLI
```shell
dotnet add package AotObjectMapper
```

## Via Visual Studio

1. Right-click your project → Manage NuGet Packages

2. Search for AotObjectMapper

3. Install the latest version

Both methods will automatically add `AotObjectMapper.Core` and `AotObjectMapper.Analyzers`.

# Quick Start

## Class definitions
```csharp
class User
{
    public required Guid           Id          { get; set; }
    public required string         FirstName   { get; set; }
    public          string?        MiddleName  { get; set; }
    public required string         LastName    { get; set; }
    public required DateTimeOffset DateOfBirth { get; set; }
    public required string         Secret      { get; set; }

}

public class UserDto
{
    public Guid     Id           { get; init; }
    public string   FirstName    { get; init; } = String.Empty;
    public string?  MiddleName   { get; init; }
    public string   LastName     { get; init; } = String.Empty;
    public int      DayOfBirth   { get; init; }
    public int      MonthOfBirth { get; init; }
    public int      YearOfBirth  { get; init; }
    public TimeOnly TimeOfBirth  { get; init; }
}
```

## Creating A Mapper

For a basic mapper all you need is a `GenerateMapper` and `Map<TSource, TDestination>` Attribute on a partial class. Below is an example for a functional mapper to convert a `User` To a `UserDto`.

```csharp
[GenerateMapper]
[Map<User, UserDto>]
public partial class UserMapper;
```

Our new mapper will automatically generate a static method called `Map` on the `UserMapper` class that returns the following when the project is built:

```csharp
new UserDto
{
    Id = source.Id,
    FirstName = source.FirstName,
    MiddleName = source.MiddleName,
    LastName = source.LastName,
};
```

The other members cannot be automatically mapped as they differ in Type and or name. Or they dont exist on one our classes. 

Multiple mapper attributes can be added to a single mapper class.
```csharp
[GenerateMapper]
[Map<User, UserDto>, Map<UserDto, User>]
public partial class UserMapper;
```

## MapperContext
For some mapping actions, a context is required to manage state, prevent issues like infinite recursion, or provide custom data. If you don't pass one to a mapping method, a new default instance is created automatically.

```csharp
var context = new MapperContext(maxDepth: 50);
        
context.AdditionalContext.Add("ExtraMetadata", ....);

var dto = UserMapper.Map(user, context);
```

The context also allows for reference preservation which is convered later.

# Customization

## Ignore Members
Specific members can be ignored for a mapping method, the example below skips the Id and MiddleName.

```csharp
[GenerateMapper]
[Map<User, UserDto>(ignoredMembers: [nameof(User.Id), nameof(User.MiddleName)])]
public partial class UserMapper;
```
## Specify Map Method Name
Map methods will automatically be named 'Map' but can be overrider as shown below.

```csharp
[GenerateMapper]
[Map<User, UserDto>(methodName: "MapUser")]
public partial class UserMapper;
```

## Null Value Handling
While Mapping between objects with required members, or when treating null warnings as errors you may get complier errors. 
To fix these either adjust the configuration to map all the appropriate members with the customization options or suppress 
these issues by passing the `SuppressNullWarnings` option flag when creating the mapper `[GenerateMapper(options: MappingOptions.SuppressNullWarnings)]`.

## IConvertable Members
Any source Member that inherits IConvertable and can convert to a destination member with the same name can be converted. Add default IFormatProviders with `UseFormatProvider` or for specific type conversions `UseFormatProvider<T1, T2>`.

```csharp
[GenerateMapper(options: MappingOptions.AllowIConvertable)]
public partial class MapperClass
{
    // Default
    [UseFormatProvider]
    private static IFormatProvider Default => CultureInfo.CurrentCulture;

    // int <-> string only
    [UseFormatProvider<int, string>]
    [UseFormatProvider<string, int>]
    private static IFormatProvider IntStringFormat => CultureInfo.InvariantCulture;
}
```

## Enums
By default enums are mapped by field name and default to a bitwise zero value if no field is found.

Override enum mapping to values with:
```csharp
[GenerateMapper(options: MappingOptions.MapEnumsByValue)]
```

And throw exceptions when no matching field is found using:
```csharp
[GenerateMapper(options: MappingOptions.ThrowExceptionOnUnmappedEnum)]
```

## Manaul Memebr Handling
To handle mapping between the diffent date of birth members the `ForMember<TSource, TDestination>(mapMethodName, memberName)` can specify methods to convert these values. `mapMethodName` will default to Map.

```csharp
[GenerateMapper(options: MappingOptions.SuppressNullWarnings)]
[Map<User, UserDto>]
[Map<UserDto, User>(methodName: "RevertMap")]
public partial class UserMapper
{
    [ForMember<User, UserDto>(nameof(UserDto.DayOfBirth))]
    public static int GetDayOfBirth(User source) => source.DateOfBirth.UtcDateTime.Day;

    [ForMember<User, UserDto>(nameof(UserDto.MonthOfBirth))]
    public static int GetMonthOfBirth(User source) => source.DateOfBirth.UtcDateTime.Month;

    [ForMember<User, UserDto>(nameof(UserDto.YearOfBirth))]
    public static int GetYearOfBirth(User source) => source.DateOfBirth.UtcDateTime.Year;

    [ForMember<User, UserDto>(nameof(UserDto.TimeOfBirth))]
    public static TimeOnly GetTimeOfBirth(User source) => new (source.DateOfBirth.UtcDateTime.TimeOfDay.Ticks);

    [ForMember<UserDto, User>(mapMethodName: nameof(RevertMap), memberName: nameof(User.DateOfBirth))]
    public static DateTimeOffset GetDateOfBirth(UserDto source, MapperContext context) 
        => new DateTimeOffset(new DateOnly(source.YearOfBirth, source.MonthOfBirth, source.DayOfBirth), source.TimeOfBirth, TimeSpan.Zero);
}
```

Methods must provide the SourceType as the first argument and optionally provide a `MapperContext` as the second.

## Nested Mapping
Nested objects can user other mappers to convert between objects using `UseMap<TMapGenerator, TSource, TDestination>`

```csharp
// Layer 1: Company Mapper
[GenerateMapper]
[Map<CompanyEntity, CompanyDto>]
[UseMap<DepartmentMapper, DepartmentEntity, DepartmentDto>]
public partial class CompanyMapper;

// Layer 2: Department Mapper
[GenerateMapper]
[Map<DepartmentEntity, DepartmentDto>]
[UseMap<EmployeeMapper, EmployeeEntity, EmployeeDto>("MapEmployee")]
public partial class DepartmentMapper;

// Layer 3: Employee Mapper
[GenerateMapper]
[Map<EmployeeEntity, EmployeeDto>("MapEmployee")]
public partial class EmployeeMapper;
```

## Reference Preservation
Circular references will infinitley try to map, adding the `PreserveReferences` flag will prevent this as well as preventing mapping the same object multiple times.

```csharp
public class Parent
{
    public Child Child { get; set; }
}

public class Child
{
    public Parent Parent { get; set; }
}

[GenerateMapper(MappingOptions.PreserveReferences)]
[Map<Parent, ParentDto>]
[UseMap<ChildMapper, Child, ChildDto>]
public partial class ParentMapper;

[GenerateMapper(MappingOptions.PreserveReferences)]
[Map<Child, ChildDto>]
[UseMap<ParentMapper, Parent, ParentDto>]
public partial class ChildMapper;
```

## IEnumerabl and Collection Types
TBD

## Pre/Post map Actions
Comming soon

## Inheritence and Interfaces
TBD