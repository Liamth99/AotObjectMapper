namespace AotObjectMapper.Tests;

public partial class NestedMapping
{
    [GenerateMapper(MappingOptions.PreserveReferences)]
    [Map<Order, OrderDto>]
    [Map<OrderDto, Order>]
    [UseMap<CustomerMapper, Customer, CustomerDto>]
    [UseMap<CustomerMapper, CustomerDto, Customer>]
    public partial class OrderMapper;

    [GenerateMapper(MappingOptions.PreserveReferences)]
    [Map<Customer, CustomerDto>]
    [Map<CustomerDto, Customer>]
    public partial class CustomerMapper;

    [GenerateMapper]
    [Map<Order, OrderDto>]
    [Map<OrderDto, Order>]
    [UseMap<CustomerMapperNoRefPreservation, Customer, CustomerDto>]
    [UseMap<CustomerMapperNoRefPreservation, CustomerDto, Customer>]
    public partial class OrderMapperNoRefPreservation;

    [GenerateMapper]
    [Map<Customer, CustomerDto>]
    [Map<CustomerDto, Customer>]
    public partial class CustomerMapperNoRefPreservation;

    [Fact]
    public void MapOrderToDto()
    {
        var order = Order.Order1();

        var dto = OrderMapper.Map(order);

        dto.Customer.ShouldNotBe(null);
        dto.Customer.Id.ShouldBe(Customer.Customer1().Id);
        dto.Customer.Email.ShouldBe(Customer.Customer1().Email);
    }

    [Fact]
    public void MapDtoToOrder()
    {
        var order = OrderDto.Order3();

        var entity = OrderMapper.Map(order);

        entity.Customer.ShouldNotBe(null);
        entity.Customer.Id.ShouldBe(CustomerDto.Customer3().Id);
        entity.Customer.Email.ShouldBe(CustomerDto.Customer3().Email);
    }

    [Fact]
    public void MapOrderToDto_ReferencesPreserved()
    {
        var context = new MapperContext();

        var order = Order.Order1();

        var dto  = OrderMapper.Map(order,                                                        ctx: context);
        var dto2 = OrderMapper.Map(new Order() { Id = 5, Customer = order.Customer, Total = 20}, ctx: context);

        ReferenceEquals(dto.Customer, dto2.Customer).ShouldBeTrue();
    }

    [Fact]
    public void MapOrderToDto_NoReferencesPreserved()
    {
        var context = new MapperContext();

        var order = Order.Order1();

        var dto  = OrderMapperNoRefPreservation.Map(order, ctx: context);
        var dto2 = OrderMapperNoRefPreservation.Map(new Order() { Id = 5, Customer = order.Customer, Total = 20}, ctx: context);

        ReferenceEquals(dto.Customer, dto2.Customer).ShouldBeFalse();
    }
}