namespace AotObjectMapper.Tests;

public partial class FactoryTests
{
    [GenerateMapper]
    [Map<User, UserDto>]
    [Map<UserDto, User>]
    public partial class FactoryUserMapper
    {
        [UseFactory<UserDto>]
        public static UserDto GetDto() => new() { FirstName = "blank", };

        [UseFactory<User>]
        public static User GetUser() => new() { FirstName = "blank", };

        [PreMap<User, UserDto>]
        public static void DtoAssertInitialValues(User _, UserDto dest)
        {
            dest.FirstName.ShouldBe("blank");
        }

        [PreMap<UserDto, User>]
        public static void UserAssertInitialValues(UserDto _, User dest)
        {
            dest.FirstName.ShouldBe("blank");
        }
    }

    [Fact]
    public void A()
    {
        var usr = User.Jim();
        var dto = FactoryUserMapper.Map(usr);
    }

    [Fact]
    public void B()
    {
        var dto = UserDto.Chris();
        var usr = FactoryUserMapper.Map(dto);
    }
}