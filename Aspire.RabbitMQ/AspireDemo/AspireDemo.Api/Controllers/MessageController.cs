using AspireDemo.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text.Json;

namespace AspireDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IConnection _connection;
        private readonly IConfiguration _configuration;

        public MessageController(IConnection connection, IConfiguration configuration)
        {
            _connection = connection;
            _configuration = configuration;
        }
        [HttpGet]
        public IResult SendMessage()
        {
            const string configName = "RabbitMQ:QueueName";
            string? queueName = _configuration[configName];

            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queueName,exclusive:false);
            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: JsonSerializer.SerializeToUtf8Bytes(new Order
                {
                    Message = "Message From API",
                    price = 100
                }));
            return Results.Ok("Message Sent..");
        }
    }
}
