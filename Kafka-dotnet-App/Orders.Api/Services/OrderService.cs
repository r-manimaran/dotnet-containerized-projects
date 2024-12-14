using Orders.Api.DTO;
using Bogus;
using Message.Contract;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Confluent.Kafka;
using System.Text.Json;
namespace Orders.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly Faker _faker;
        private readonly IProducer<Null,string> _producer;

        private List<OrderRequest> Orders = new List<OrderRequest>();
        public OrderService(ILogger<OrderService> logger,                            
                            IProducer<Null,string> producer)
        {
            _logger = logger;
            _faker = new Faker();
            _producer = producer;
        }
        public async Task<bool> CreateOrder(OrderRequest Order)
        {
            //Handle the logic to save it in database
            Orders.Add(Order);
            _logger.LogInformation("Order Created Successfully.");

            // Handle the Message Contract
            var newOrderMessage = new OrderReceivedMessage
            {
                Id = _faker.Random.Int(1, 1000),
                OrderId = _faker.Random.Int(1000, 5000),
                CustomerEmail = _faker.Person.Email,
                CustomerId = _faker.Random.Int(5000, 6000),                                
                Products = GetProducts(_faker.Random.Int(1,6)),
                TotalPrice = _faker.Random.Decimal(20, 50),
                OrderDate = _faker.Date.Recent()               
            };

            var kafkaPublishResult = await _producer.ProduceAsync("new-order-topic",
                                                         new Message<Null, string> 
                                                         {
                                                            Value = JsonSerializer.Serialize(newOrderMessage) 
                                                         });

            if (kafkaPublishResult.Status != PersistenceStatus.Persisted)
            {
                //There is some error
                _logger.LogError("Unable to publish the message to Kafka");
            }
            else
            {
                _logger.LogInformation("Successfully published the message");
            }

            return true;

        }

        private List<OrderProduct> GetProducts(int randomNumber)
        {
            List<OrderProduct> products = new List<OrderProduct>();
            for(int i = 0; i < randomNumber; i++)
            {
                OrderProduct product = new OrderProduct();
                product.Id = _faker.Random.Int(0, 50);
                product.Name = _faker.Commerce.ProductName();
                product.Quantity = _faker.Random.Int(1, 5);
                product.Price = _faker.Random.Decimal(20, 50);
                products.Add(product);
            }
            
            return products;
        }
    }
}
