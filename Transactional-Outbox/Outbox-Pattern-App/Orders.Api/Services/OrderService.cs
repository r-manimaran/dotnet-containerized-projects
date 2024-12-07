using Bogus;
using MassTransit;
using Message.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using Orders.Api.DTOs;
using Orders.Api.Models;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Orders.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly AppDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly Faker _faker;
        private readonly Random _random;
        public OrderService(ILogger<OrderService> logger, AppDbContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;

            _faker = new Faker();
            _random = new Random();
        }
        public async Task<OrderPublish> CreateNewOrderAsync(OrderRequest orderRequest)
        {
            _logger.LogInformation($"Received the new Order");
            // Added the Transacation
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {


                var newOrder = new Order
                {
                    CustomerId = orderRequest.CustomerId,
                    TotalPrice = orderRequest.TotalPrice,
                    CreatedDate = DateTime.UtcNow,
                };
                await _dbContext.Orders.AddAsync(newOrder);
                await _dbContext.SaveChangesAsync();

                var orderPublish = new OrderPublish()
                {
                    OrderId = newOrder.Id,
                    CustomerId = orderRequest.CustomerId,
                    CustomerName = _faker.Person.FullName,
                    CustomerEmail = _faker.Person.Email,
                    CustomerPhone = _faker.Person.Phone,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = orderRequest.TotalPrice,
                };
                _logger.LogInformation($"Order Created Successfully");

                // publish to RabbitMQ
                // Now the message will go to Outbox message instead of direct RabbitMQ
                // await _publishEndpoint.Publish(orderPublish);
                await _dbContext.InsertOutboxMessage(orderPublish);
                await _dbContext.SaveChangesAsync();

                // commit the transacation
                await transaction.CommitAsync();
                _logger.LogInformation("Order Published Successfully with orderId:{OrderId}",newOrder.Id);
                return orderPublish;
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                await transaction.RollbackAsync();
                
                _logger.LogError(ex, "Error occurred while creating order");
                throw;
            }
        }

        public async Task<OrderPublish> GetOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Retrieving order with ID: {OrderId}", orderId);

            // Use async/await properly with FirstOrDefaultAsync
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(item => item.Id == orderId);

            if (order is null)
            {
                _logger.LogWarning("Order not found with ID: {OrderId}", orderId);
                throw new Exception($"Order with ID {orderId} was not found");
            }


            return new OrderPublish
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = _faker.Person.FullName,
                CustomerEmail = _faker.Person.Email,
                CustomerPhone = _faker.Person.Phone,
                OrderDate = order.CreatedDate,
                TotalPrice = order.TotalPrice
            };

        }


    }
}
