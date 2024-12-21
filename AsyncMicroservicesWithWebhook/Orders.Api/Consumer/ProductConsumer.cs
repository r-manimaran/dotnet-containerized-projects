using MassTransit;
using Shared.Contracts.Models;

namespace Orders.Api.Consumer
{
    public class ProductConsumer : IConsumer<Product>
    {
        private readonly OrderDbContext _orderDbContext;

        public ProductConsumer(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }
        public async Task Consume(ConsumeContext<Product> context)
        {
            _orderDbContext.Products.Add(context.Message);
            await _orderDbContext.SaveChangesAsync();
        }
    }
}
