namespace AotObjectMapper.Sample.Models;
#pragma warning disable LOCAT001

public class Customer
{
    public int    Id    { get; set; }
    public string Email { get; set; } = string.Empty;

    public static Customer Customer1()
    {
        return new Customer()
        {
            Id = 1, Email = "email_1@somedomain.com"
        };
    }

    public static Customer Customer2()
    {
        return new Customer()
        {
            Id = 2, Email = "email_2@somedomain.com"
        };
    }
}

public class CustomerDto
{
    public int    Id    { get; set; }
    public string Email { get; set; } = string.Empty;

    public static Customer Customer3()
    {
        return new Customer()
        {
            Id = 3, Email = "email_3@somedomain.com"
        };
    }

    public static Customer Customer4()
    {
        return new Customer()
        {
            Id = 4, Email = "email_4@somedomain.com"
        };
    }
}