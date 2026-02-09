namespace AotObjectMapper.Tests;

public partial class ForMemberMapping
{

    [GenerateMapper]
    [Map<User, UserAltDto>]
    [Map<UserAltDto, User>]
    public partial class UserForMemberMapper
    {
        // DTO -> User
        [ForMember<UserAltDto, User>(nameof(User.FirstName))]
        private static string GetFirstName(UserAltDto source) => source.GivenName;

        [ForMember<UserAltDto, User>(nameof(User.LastName))]
        private static string GetLastName(UserAltDto source) => source.FamilyName;

        // User -> DTO
        [ForMember<User, UserAltDto>(nameof(UserAltDto.GivenName))]
        private static string GetGivenName(User source) => source.FirstName;

        [ForMember<User, UserAltDto>(nameof(UserAltDto.FamilyName))]
        private static string GetFamilyName(User source) => source.LastName;
    }

    [Fact]
    public void MapUserToAltDto()
    {
        var user = User.Jim();

        UserAltDto dto = UserForMemberMapper.Map(user);

        dto.Id.ShouldBe(user.Id);
        dto.Age.ShouldBe(user.Age);
        dto.GivenName.ShouldBe(user.FirstName);
        dto.FamilyName.ShouldBe(user.LastName);
    }

    [Fact]
    public void MapAltDtoToUser()
    {
        var user = UserAltDto.Anna();

        User entity = UserForMemberMapper.Map(user);

        entity.Id.ShouldBe(user.Id);
        entity.Age.ShouldBe(user.Age);
        entity.FirstName.ShouldBe(user.GivenName);
        entity.LastName.ShouldBe(user.FamilyName);
    }
}