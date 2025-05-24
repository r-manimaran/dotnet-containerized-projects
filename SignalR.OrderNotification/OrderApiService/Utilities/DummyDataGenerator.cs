using Bogus;
using OrderApiService.Models;

namespace OrderApiService.Utilities;

public class DummyDataGenerator
{
    // Generate a dummy order with random data
    public static Order GenerateDummyOrderManual()
    {
        // Generate a dummy order with random data
        var order = new Order();
        order.Id = Guid.NewGuid();
        order.CustomerName = "John Doe";
        order.CustomerEmail = "johndoe@example.com";
        order.OrderItems = new List<OrderItem>
        {
            new OrderItem { ProductName = "Wireless Mouse", Quantity = 2, UnitPrice = 10.99m },
            new OrderItem { ProductName = "Mechanical Keyborad", Quantity = 1, UnitPrice = 5.49m },
            new OrderItem { ProductName = "USB-C Hub", Quantity = 1, UnitPrice = 15.99m }
        };
        order.Status = OrderStatus.Created;
        order.UpdatedAt = DateTime.UtcNow;
        return order;
    }

    public static Order GenerateDummyOrder()
    {
        // Faker for OrderItem
        var orderItemFaker = new Faker<OrderItem>()
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 5))
            .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(5, 100));

        // Faker for Order
        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.Id, f => Guid.NewGuid())
            .RuleFor(o => o.CustomerName, f => f.Name.FullName())
            .RuleFor(o => o.CustomerEmail, f => f.Internet.Email())
            .RuleFor(o => o.OrderItems, f => orderItemFaker.Generate(f.Random.Int(1, 5)))
            .RuleFor(o => o.Status, OrderStatus.Created)
            .RuleFor(o => o.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(o => o.UpdatedAt, f => f.Date.Recent());

        return orderFaker.Generate();
    }
}
