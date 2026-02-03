using System;
using System.Text.Json;

public class Program()
{
    public static void Main(string[] args)
    {
        // Use this file to test and debug the generator by running the DebugRoslynSourceGenerator launch profile.

        var dto = UserMapper.Map(User.Jim());

        Console.WriteLine(JsonSerializer.Serialize(dto, new JsonSerializerOptions(){ WriteIndented = true }));
    }
}

namespace AotObjectMapper.Sample
{

    [GenerateMapper]
    [Map<User, UserAltDto>]
    public partial class UserMapper
    {
        [ForMember<User, UserAltDto>(nameof(UserAltDto.GivenName))]
        private static string GetGivenName(User source) => source.FirstName;

        [ForMember<User, UserAltDto>(nameof(UserAltDto.FamilyName))]
        private static string GetFamilyName(User source) => source.LastName;
    }
}