using System.Reflection;

// ReSharper disable InconsistentNaming

namespace AotObjectMapper.Tests;

[GenerateMapper]
[UseMap<UserMapper, User, UserDto>]
[Map<Company, CompanyDto>]
public partial class CompanyMapper;

[GenerateMapper]
[UseMap<UserMapper, User, UserDto>]
[Map<Company, CompanyDto_Arr>]
public partial class CompanyDto_ArrMapper;

[GenerateMapper]
[UseMap<UserMapper, User, UserDto>]
[Map<Company, CompanyDto_List>]
public partial class CompanyDto_ListMapper;

[GenerateMapper]
[UseMap<UserMapper, User, UserDto>]
[Map<Company, CompanyDto_Collection>]
public partial class CompanyDto_CollectionMapper;

[GenerateMapper]
[UseMap<UserMapper, User, UserDto>]
[Map<Company, CompanyDto_ReadOnlyList>]
public partial class CompanyDto_ReadOnlyListMapper;

[GenerateMapper]
[UseMap<UserMapper, User, UserDto>]
[Map<Company, CompanyDto_ReadOnlyCollection>]
public partial class CompanyDto_ReadOnlyCollectionMapper;

public class Company    { public IEnumerable<User> Users { get; set; } = [User.Jim(), User.Jim(),]; }

public class CompanyDto                    { public IEnumerable<UserDto>         Users { get; set; } = []; }
public class CompanyDto_Arr                { public UserDto[]                    Users { get; set; } = []; }
public class CompanyDto_List               { public List<UserDto>                Users { get; set; } = []; }
public class CompanyDto_Collection         { public ICollection<UserDto>         Users { get; set; } = []; }
public class CompanyDto_ReadOnlyList       { public IReadOnlyList<UserDto>       Users { get; set; } = []; }
public class CompanyDto_ReadOnlyCollection { public IReadOnlyCollection<UserDto> Users { get; set; } = []; }


public class IEnumerableTests
{
    [Theory]
    [InlineData(typeof(CompanyMapper),                       typeof(IEnumerable<UserDto>))]
    [InlineData(typeof(CompanyDto_ArrMapper),                typeof(UserDto[]))]
    [InlineData(typeof(CompanyDto_ListMapper),               typeof(List<UserDto>))]
    [InlineData(typeof(CompanyDto_CollectionMapper),         typeof(ICollection<UserDto>))]
    [InlineData(typeof(CompanyDto_ReadOnlyListMapper),       typeof(IReadOnlyList<UserDto>))]
    [InlineData(typeof(CompanyDto_ReadOnlyCollectionMapper), typeof(IReadOnlyCollection<UserDto>))]
    public void Map_Should_Map_To_ExpectedType(Type mapper, Type expectedType)
    {
        var src = new Company();

        var method = mapper.GetMethod("Map", BindingFlags.Static | BindingFlags.Public)!;

        dynamic dto = method.Invoke(null, [src, null])!;

        var users = (IEnumerable<UserDto>)dto.Users;

        users.ShouldBeAssignableTo(expectedType);
        users.Count().ShouldBe(src.Users.Count());

    }
}