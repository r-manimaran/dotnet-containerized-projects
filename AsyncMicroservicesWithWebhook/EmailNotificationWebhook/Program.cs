using EmailNotificationWebhook.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.DTOs;
using MassTransit;
using EmailNotificationWebhook.Consumer;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddMassTransit(x=>
{
    x.AddConsumer<EmailWebhookConsumer>();
    x.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbitmq://localhost", c =>
        {
            c.Username("guest");
            c.Password("guest");
        });
        config.ReceiveEndpoint("email-webhook-queue", e =>
        {
            e.ConfigureConsumer<EmailWebhookConsumer>(context);
        });
    });
});

var app = builder.Build();

app.MapPost("/email-webhook", ([FromBody]EmailDTO email, 
                                    IEmailService _emailService) =>
{
    string result =_emailService.SendEmail(email);
    return Task.FromResult(result);
});

app.Run();
