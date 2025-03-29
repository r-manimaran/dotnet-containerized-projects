using Aspire.BackendApi;
using RabbitMQ.Client;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.AddRabbitMQClient("messaging");

builder.Services.AddHttpClient<ExternalApiClient>(client=> client.BaseAddress =
        new Uri("https://external-api"));

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();

app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "Open API V1"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/send-hit", async (ExternalApiClient externalApiClient) =>
{
    try
    {
        await externalApiClient.SendHit("Sample Text from .Net Aspire");
        return " A hit was sent to external API";
    }
    catch (Exception ex)
    {
        return $"An error occurred during request external API: {ex?.Message}";
    }
});

app.MapPost("/post-message", async (IConnection connection) =>
{
    try
    {
        var queueName = "test-queue";
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queueName, exclusive: false, durable: true);
        channel.BasicPublish(exchange: "", queueName, null,
                    body: JsonSerializer.SerializeToUtf8Bytes(
                        new
                        {
                            Title = "Book 01",
                            Description = "Description of Book 01"
                        }));
        return "Message was send to RabbitMQ queue";
    }
    catch(Exception ex)
    {
        return $"An Error occured when publishing the message to Queue:{ex?.Message}";
    }
});

app.Run();
