using System.Globalization;

namespace AotObjectMapper.Tests;

[GenerateMapper(MappingOptions.AllowIConvertable)]
[Map<User, UserStringDto>]
[Map<UserStringDto, User>]
public partial class UserIConvertableMapper
{
    [UseFormatProvider]
    public static IFormatProvider Default => CultureInfo.InvariantCulture;
}

public class IConvertableMapping
{
    [Fact]
    public void MapUserToStringDto()
    {
        var user = User.Jim();

        UserStringDto dto = UserIConvertableMapper.Map(user);

        dto.Id.ShouldBe(Convert.ToString(user.Id,   UserIConvertableMapper.Default));
        dto.Age.ShouldBe(Convert.ToString(user.Age, UserIConvertableMapper.Default));
        dto.FirstName.ShouldBe(user.FirstName);
        dto.LastName.ShouldBe(user.LastName);
    }

    [Fact]
    public void MapStringDtoToUser()
    {
        var user = UserStringDto.Isabel();

        User entity = UserIConvertableMapper.Map(user);

        entity.Id.ShouldBe(Convert.ToInt32(user.Id,    UserIConvertableMapper.Default));
        entity.Age.ShouldBe(Convert.ToSingle(user.Age, UserIConvertableMapper.Default));
        entity.FirstName.ShouldBe(user.FirstName);
        entity.LastName.ShouldBe(user.LastName);
    }
}