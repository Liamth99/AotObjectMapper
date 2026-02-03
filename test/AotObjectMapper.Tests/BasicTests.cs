namespace AotObjectMapper.Tests;


[GenerateMapper]
[Map<User, UserDto>]
[Map<UserDto, User>]
public partial class UserMapper;

public class BasicTests
{

    [Fact]
    public void MapUserToDto()
    {
        var user = User.Jim();

        UserDto dto = UserMapper.Map(user);

        dto.Id.ShouldBe(user.Id);
        dto.Age.ShouldBe(user.Age);
        dto.FirstName.ShouldBe(user.FirstName);
        dto.LastName.ShouldBe(user.LastName);
    }

    [Fact]
    public void MapDtoToUser()
    {
        var user = UserDto.Chris();

        User entity = UserMapper.Map(user);

        entity.Id.ShouldBe(user.Id);
        entity.Age.ShouldBe(user.Age);
        entity.FirstName.ShouldBe(user.FirstName);
        entity.LastName.ShouldBe(user.LastName);
    }
}