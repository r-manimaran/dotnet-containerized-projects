using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.DTOs;
using Shared.Contracts.Models;
using System.Text;

namespace Orders.Api.Repositories;

public class OrderRepository : IOrder
{
    private readonly ILogger<OrderRepository> _logger;
    private readonly OrderDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderRepository(ILogger<OrderRepository> logger, 
                           OrderDbContext dbContext,
                           IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _dbContext = dbContext;
       _publishEndpoint = publishEndpoint;
    }
    public async Task<ServiceResponse> AddOrderAsync(OrderRequest order)
    {
       
        var newOrder = new Order()
        {
            OrderedDate = DateTime.UtcNow,
            TotalPrice = order.Products.Sum(t=>t.Quantity*t.Price) 
        };
        _dbContext.Orders.Add(newOrder);
        await _dbContext.SaveChangesAsync();

        foreach (var orderItem in order.Products)
        {
            var orderProduct = new OrderProduct
            {
                OrderId = newOrder.Id,
                ProductId = orderItem.ProductId,
                Price = orderItem.Price,
                Quantity = orderItem.Quantity,
            };
          _dbContext.OrderProducts.Add(orderProduct);
        }        
        
       
        await _dbContext.SaveChangesAsync();

        var orderSummary = await GetOrderSummaryAsync();
        _logger.LogInformation("Create the Email Content for the Order");
        string emailContent = BuildOrderEmailBody(
                                                 orderSummary.Id, 
                                                 orderSummary.Products, 
                                                 orderSummary.TotalAmount, 
                                                 orderSummary.OrderedDate);
        _logger.LogInformation("Publish to RabbitMQ");
        await _publishEndpoint.Publish(new EmailDTO("New Order Placed Successfully.", emailContent));

        return new ServiceResponse(true, "Order Created Successfully.");
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var orders = await _dbContext.Orders.ToListAsync();
        return orders;
    }

    public async Task<OrderSummary> GetOrderSummaryAsync()
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync();
        var products = await _dbContext.Products.ToListAsync();
        List<ProductList> productLists = new List<ProductList>();

        foreach (var product in order!.Products) 
        {
            var dbproduct = products.Where(p=>p.Id ==  product.Id).FirstOrDefault();
            if (dbproduct != null)
            {
                ProductList productList = new ProductList(dbproduct.Id, dbproduct.Name, dbproduct.Price, product.Quantity);
                productLists.Add(productList);
            }
        }

        return new OrderSummary(order.Id, productLists, order.TotalPrice, order.OrderedDate);
    }

    private string BuildOrderEmailBody(int orderId, List<ProductList> products, decimal totalAmount, DateTime orderDate)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<h1><strong>Order Information</strong></h1>");
        sb.AppendLine($"Order Id:{orderId}");
        sb.AppendLine($"Order Date:{orderDate.ToShortDateString()}");
        sb.AppendLine("<h2>Product | Quanity | Price | Total Price|</h2>");
        foreach(var product in products)
        {
            sb.AppendLine(product.ProductName + " " +product.Quantity+" " + product.Price +" "+(product.Quantity*product.Price));
        }
        sb.AppendLine($"Total Price:{totalAmount}");
        sb.AppendLine("<p>Thanks for your Order.</p>");
        return sb.ToString();
    }
}
