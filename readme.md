# AotObjectMapper
[![NuGet Version](https://img.shields.io/nuget/v/AotObjectMapper.Mapper.svg)](https://www.nuget.org/packages/AotObjectMapper.Mapper/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AotObjectMapper.Mapper.svg)](https://www.nuget.org/packages/AotObjectMapper.Mapper/)
[![GitHub Release](https://img.shields.io/github/v/release/Liamth99/AotObjectMapper)](https://github.com/Liamth99/AotObjectMapper/releases)
[![License](https://img.shields.io/github/license/Liamth99/AotObjectMapper)](https://github.com/Liamth99/AotObjectMapper/blob/master/LICENSE)

AotObjectMapper is a C# Roslyn source generator for compile-time object mapping.

This project was created primarily as a personal learning exercise and proof of concept. And may not be suitable for production use until v1.0.0

The main goals of this project are:
- Gaining hands-on experience with Roslyn incremental source generators
- Designing an AOT- and trimming-friendly alternative to reflection-based mappers
- Providing feature parity with mature mapping libraries such as AutoMapper or Mapster
- Exploring compile-time code generation, diagnostics, and analyzers
- Experimenting with API design and developer experience in attribute driven tooling
- Handling complex mapping scenarios such as recursion, reference preservation, enum strategies, and custom member mappings

<p align="center">
<img src="/docs/static/banner1280x640.png" alt="AotObjectMapper banner" height="256">
</p>

# Installation

AotObjectMapper is distributed as a [NuGet package](https://www.nuget.org/packages/AotObjectMapper.Mapper/).

## Via .NET CLI
```shell
dotnet add package AotObjectMapper
```

## Via Visual Studio

1. Right-click your project → Manage NuGet Packages

2. Search for AotObjectMapper

3. Install the latest version

Both methods will automatically add `AotObjectMapper.Abstractions`.

# Quick Start
```csharp
[GenerateMapper]
[Map<User, UserDto>]
public partial class UserMapper;
```
When the project is built, the mapper will generate a static `Map` method on `UserMapper` which will map Id, FirstName, MiddleName and LastName.

The remaining members cannot be mapped automatically because they differ in type and/or name, or do not exist on one of the classes. This can
be fixed using mapper configuration which is all covered in the documentation.

Multiple Map attributes can be added to a single mapper class:
```csharp
[GenerateMapper]
[Map<User, UserDto>, Map<UserDto, User>]
public partial class UserMapper;
```
For more information visit the [Docs](https://liamth99.github.io/AotObjectMapper/)