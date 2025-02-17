using Aspire.Database.TestContainers.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspire.Database.TestContainers.Models;
using System.Net.Http.Json;
using RabbitMQ.Client;

namespace Aspire.Database.TestContainers.Testing;

public class TodoTests : BaseIntegrationTest
{
    public TodoTests(ApiFactory factory) : base(factory)
    {
    }
    [Fact]
    public async Task Should_Publish_event_when_todo_Created()
    {
        var todo = new Todo
        {
            Id = 0,
            Task = "Test RabbitMQ using TestContainer",
            DueDate = DateTime.Now.AddDays(1),
            IsCompleted = false
        };

        await _apiClient.PostAsJsonAsync("/api/ToDos", todo);

       // Assert.True(_connection.IsOpen);

    }
}
