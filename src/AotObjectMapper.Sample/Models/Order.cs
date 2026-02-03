namespace AotObjectMapper.Sample.Models;
#pragma warning disable LOCAT001

public class Order
{
    public          int      Id       { get; set; }
    public required Customer Customer { get; set; }
    public          decimal  Total    { get; set; }

    public static Order Order1()
    {
        return new Order()
        {
            Id = 1, Customer = Customer.Customer1(), Total = 100
        };
    }

    public static Order Order2()
    {
        return new Order()
        {
            Id = 2, Customer = Customer.Customer2(), Total = 200
        };
    }
}

public class OrderDto
{
    public          int         Id       { get; set; }
    public required CustomerDto Customer { get; set; }
    public          decimal     Total    { get; set; }

    public static Order Order3()
    {
        return new Order()
        {
            Id = 3, Customer = CustomerDto.Customer3(), Total = 300
        };
    }

    public static Order Order2()
    {
        return new Order()
        {
            Id = 4, Customer = CustomerDto.Customer4(), Total = 400
        };
    }
}