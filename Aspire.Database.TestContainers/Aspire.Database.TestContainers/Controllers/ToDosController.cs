using Aspire.Database.TestContainers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Aspire.RabbitMQ.Client;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

namespace Aspire.Database.TestContainers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDosController : ControllerBase
    {
        private readonly ILogger<ToDosController> _logger;
        private readonly IConnection _connection;
        private IModel _channel;
        public ToDosController(ILogger<ToDosController> logger, IConnection connection)
        {
            _logger = logger;
            _connection = connection;
            _channel = _connection.CreateModel();
        }

        [HttpPost]
        public async Task<IActionResult> CreateToDo(Todo todo)
        {
            _channel.QueueDeclare("todoqueue",durable:false, exclusive:false, autoDelete:false, arguments:null);
            var messageJson = JsonConvert.SerializeObject(todo);
            var body = Encoding.UTF8.GetBytes(messageJson);
            await Task.Run(() => _channel.BasicPublish(exchange: "", routingKey: "todoqueue", basicProperties: null, body: body));

            return Ok(body);
        }
    }
}
