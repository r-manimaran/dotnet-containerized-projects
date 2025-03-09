using eshop.Orders.Modules.Orders.Models;
using eshop.Orders.Modules.Orders.PublicApi;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

namespace eshop.Orders.Modules.Orders
{
    public static class Endpoints
    {
        public static void MapOrdersEndpoints(this IEndpointRouteBuilder route)
        {
            var app = route.MapGroup("api/orders")
                .WithOpenApi(operation=> new OpenApiOperation(operation)
                    {
                        Summary = "Order Management API", // Group-level summary
                        Description = "APIs for creating and retrieving orders" // Group level description
                    })
                .WithTags("Orders");

            app.MapPost("", async (CreateOrderDto order, IOrderService orderService) =>
            {
                var result = await orderService.CreateOrder(order);

                return Results.Created($"orders/{result.Id}", result);
            })
            .WithName("createOrder") // Named endpoint for URL generation
            .WithDescription("Create a new order in the system") // Endpoint description
            .WithSummary("Create a new Order") // Brief Summary
            .WithOpenApi(operation =>
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = { ["application/json"]=new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "CreateOrderDto" // References to DTO schema
                            }
                        }
                    }
                }
                };
                return operation;
            })
            .Produces<Order>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

            app.MapGet("/{id:guid}", async (Guid id, IOrderService orderService) =>
            {
                var order = await orderService.GetOrder(id);
                return order is null ? Results.NotFound() : Results.Ok(order);                
            });
        }
    }
}
