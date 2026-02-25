namespace AotObjectMapper.Tests;

public partial class MappingHookTests
{
    [GenerateMapper]
    [Map<User, UserDto>]
    [Map<UserDto, User>]
    public partial class HookedUserMapper
    {
        // ***************************************************************************
        // Note these are very bad examples of pre/post mapping hooks, don't reproduce
        // ***************************************************************************

        [PreMap<User, UserDto>]
        private static void PreMapUserDto(User src, UserDto dest)
        {
            src.FirstName = "[redacted]";
        }

        [PostMap<User, UserDto>]
        private static void PostMapUserDto(UserDto dest)
        {
            dest.FirstName = "was redacted";
        }

        [PreMap<UserDto, User>]
        private static void PreMapUser(UserDto src, User dest, MapperContextBase ctx)
        {
            src.FirstName = (string)ctx.AdditionalContext["redactmsg"];
            ctx.AdditionalContext["redactmsg"] = "was used";
        }

        [PostMap<UserDto, User>]
        private static void PostMapUser(User dest)
        {
            dest.FirstName = "was redacted";
        }
    }

    [Fact]
    public void NoContextHooksRuns()
    {
        var src = User.Jim();
        var dto = HookedUserMapper.Map(src);

        src.FirstName.ShouldBe("[redacted]");
        dto.FirstName.ShouldBe("was redacted");
    }

    [Fact]
    public void ContextHooksRuns()
    {
        var ctx = new MapperContext();
        ctx.AdditionalContext.Add("redactmsg", "[redacted]");

        var src = UserDto.Chris();
        var dto = HookedUserMapper.Map(src, ctx);

        ctx.AdditionalContext.ShouldContainKeyAndValue("redactmsg", "was used");

        src.FirstName.ShouldBe("[redacted]");
        dto.FirstName.ShouldBe("was redacted");
    }
}