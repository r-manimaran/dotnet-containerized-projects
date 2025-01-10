using BasketApi.Dtos;
using BasketApi.Models;
using Carter;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Repository;

namespace BasketApi.Endpoints;

public class BasketEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/baskets").WithTags("Basket");

        app.MapPost("", CreateBasketRequest);
       
    }

    private async Task CreateBasketRequest([FromBody] ConfirmBasketDto request, 
                                        IRepository<Basket> repository, ILogger<Basket> logger)
    {

        logger.LogInformation("Received the new bucket item");


    }
}
