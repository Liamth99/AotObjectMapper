# AotObjectMapper
[![NuGet Version](https://img.shields.io/nuget/v/AotObjectMapper.svg)](https://www.nuget.org/packages/AotObjectMapper/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AotObjectMapper.svg)](https://www.nuget.org/packages/AotObjectMapper/)
[![GitHub Release](https://img.shields.io/github/v/release/Liamth99/AotObjectMapper)](https://github.com/Liamth99/AotObjectMapper/releases)
[![License](https://img.shields.io/github/license/Liamth99/AotObjectMapper)](https://github.com/Liamth99/AotObjectMapper/blob/master/LICENSE)

AotObjectMapper is a C# Roslyn source generator for compile-time object mapping.

This project was created primarily as a personal learning exercise and proof of concept. And may not be suitable for production use until v1.0.0

<p align="center">
<img src="/banner1280x640.png" alt="AotObjectMapper banner" height="256">
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
    public Guid     Id           { get; set; }
    public string   FirstName    { get; set; } = String.Empty;
    public string?  MiddleName   { get; set; }
    public string   LastName     { get; set; } = String.Empty;
    public int      DayOfBirth   { get; set; }
    public int      MonthOfBirth { get; set; }
    public int      YearOfBirth  { get; set; }
    public TimeOnly TimeOfBirth  { get; set; }
}
```

## Creating A Mapper

For a basic mapper, all you need is a `GenerateMapper` and one or more `Map<TSource, TDestination>` attributes on a partial class.

Below is an example mapper that converts a User to a UserDto.

```csharp
[GenerateMapper]
[Map<User, UserDto>]
public partial class UserMapper;
```
When the project is built, the mapper will generate a static `Map` method on `UserMapper` which will map Id, FirstName, MiddleName and LastName.

The remaining members cannot be mapped automatically because they differ in type and/or name, or do not exist on one of the classes.

Multiple Map attributes can be added to a single mapper class:
```csharp
[GenerateMapper]
[Map<User, UserDto>, Map<UserDto, User>]
public partial class UserMapper;
```

## MapperContext
Some mapping scenarios require a context to manage state, prevent issues such as infinite recursion, or provide additional data.
If no context is passed to a mapping method, a default instance is created automatically.

```csharp
var context = new MapperContext(maxDepth: 50);
        
context.AdditionalContext.Add("ExtraMetadata", ....);

var dto = UserMapper.Map(user, context);
```

The context also supports reference preservation, which is covered later.

# Customization

## Ignore Members
Specific members can be ignored for a mapping. The example below skips `Id` and `MiddleName`.

```csharp
[GenerateMapper]
[Map<User, UserDto>(ignoredMembers: [nameof(User.Id), nameof(User.MiddleName)])]
public partial class UserMapper;
```
## Specify Map Method Name
Map methods are named `Map` by default but can be overridden as shown below.

```csharp
[GenerateMapper]
[Map<User, UserDto>(methodName: "MapUser")]
public partial class UserMapper;
```

## Null Value Handling
When mapping between objects with required members, or when treating nullable warnings as errors, you may encounter compiler errors.

To resolve this, either configure mappings for all required members using customization options, or suppress these warnings by passing the `SuppressNullWarnings` flag:

## IConvertable Members
Any source member that implements `IConvertible` and can convert to a destination member with the same name can be mapped automatically.

Add default format providers with `UseFormatProvider`, or specify providers for specific conversions using `UseFormatProvider<TSource, TDestination>`.

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
By default, enums are mapped by field name and fall back to the zero value if no matching field is found.

Map enums by value instead:
```csharp
[GenerateMapper(options: MappingOptions.MapEnumsByValue)]
```

Throw an exception when no matching enum value is found:
```csharp
[GenerateMapper(options: MappingOptions.ThrowExceptionOnUnmappedEnum)]
```
note: this does nothing if `MapEnumsByValue` is set.

## Manaul Memeber Handling
To map between members that differ (such as the date-of-birth fields), use `ForMember<TSource, TDestination>` to specify custom conversion methods.
mapMethodName defaults to Map.

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

Methods must take the source type as the first parameter and may optionally accept a `MapperContext` as the second.

## Nested Mapping
Nested objects can use other mappers via `UseMap<TMapGenerator, TSource, TDestination>`

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
Circular references can cause infinite mapping loops. Enabling `PreserveReferences` prevents this and ensures the same object is not mapped multiple times.

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

## IEnumerable and Collection Types
TBD

## Pre/Post map Actions
Comming soon

## Inheritence and Interfaces
TBD