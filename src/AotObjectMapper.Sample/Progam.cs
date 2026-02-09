using System;
using System.Text.Json;

namespace Sample;

public partial class Program()
{
    // *******************************************************************************************************
    // Use this file to test and debug the generator by running the DebugRoslynSourceGenerator launch profile.
    // *******************************************************************************************************

    [GenerateMapper]
    [Map<User, UserDto>]
    public partial class UserMapper;

    public static void Main(string[] args)
    {
        var src = User.Jim();

        var dto = UserMapper.Map(src);

        Console.WriteLine($"{src.GetType().Name}:");
        Console.WriteLine(JsonSerializer.Serialize(src, new JsonSerializerOptions(){ WriteIndented = true }));
        Console.WriteLine();
        Console.WriteLine($"{dto.GetType().Name}:");
        Console.WriteLine(JsonSerializer.Serialize(dto, new JsonSerializerOptions(){ WriteIndented = true }));
    }
}