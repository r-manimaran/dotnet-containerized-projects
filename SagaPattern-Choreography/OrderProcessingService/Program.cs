using OrderProcessingService.Services;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.Threading.TaskParallelLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRebus(
    configure => configure
        .Routing(
              routing => routing.TypeBased().MapAssemblyOf<Program>("Rebus.OrderQueue"))
        .Transport(transport =>
                transport.UseRabbitMq(
                        builder.Configuration.GetConnectionString("RabbitMq"),
                         "Rebus.OrderQueue"))
        .Sagas(sagas =>
                  sagas.StoreInMemory())
        );

builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AutoRegisterHandlersFromAssemblyOf<Program>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
