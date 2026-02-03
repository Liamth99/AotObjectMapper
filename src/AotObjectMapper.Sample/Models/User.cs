namespace AotObjectMapper.Sample.Models;
#pragma warning disable LOCAT001

public class User
{
    public int    Id        { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public float  Age       { get; set; }

    public static User Jim()
    {
        return new User()
        {
            Id = 1, Age = 30, FirstName = "Jim", LastName = "Jimmerson"
        };
    }
}

public class UserDto
{
    public int    Id        { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public float  Age       { get; set; }

    public static UserDto Chris()
    {
        return new UserDto()
        {
            Id = 2, Age = 35, FirstName = "Chris", LastName = "Christensen"
        };
    }
}

public class UserStringDto
{
    public string Id        { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; }  = string.Empty;
    public string Age       { get; set; } = string.Empty;

    public static UserStringDto Isabel()
    {
        return new UserStringDto()
        {
            Id = "3", Age = "25", FirstName = "Isabel", LastName = "Isabelensen"
        };
    }
}

public class UserAltDto
{
    public int    Id         { get; set; }
    public string GivenName  { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public float  Age        { get; set; }

    public static UserAltDto Anna()
    {
        return new UserAltDto()
        {
            Id = 4, Age = 33, GivenName = "Anna", FamilyName = "Annaerson"
        };
    }
}