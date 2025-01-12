using BasketApi.Dtos;
using BasketApi.Models;
using Carter;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Enums;
using SharedLib.Repository;
using SharedLib.Sagas.Events;
using SharedLib.Sagas.Messages;

namespace BasketApi.Endpoints;

public class BasketEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/baskets").WithTags("Basket");

        group.MapPost("", CreateBasketRequest);
       
    }

    private async Task<IResult> CreateBasketRequest(
                                        [FromBody] ConfirmBasketDto request, 
                                        IRepository<Basket> repository, 
                                        IPublishEndpoint publishEndpoint,
                                        ILogger<Basket> logger)
    {

        logger.LogInformation("Received the new bucket item");
        var basket = Basket.Create(request.CustomerId, SharedLib.Enums.BasketStatus.Uncertain);

        request.BasketItems.ForEach(basketItem => 
            basket.AddBasketItem(basketItem.ProductId, basket.Id, basketItem.Price, basketItem.Count, BasketStatus.Uncertain));
       
        await repository.AddAsync(basket);
        await repository.SaveChangesAsync();
        logger.LogInformation("Created new bucket item");

        var basketConfirmedEvent = new BasketConfirmedEvent(
            BasketId:basket.Id,
            CustomerId : basket.CustomerId,
            PaymentMessage :request.Payment.Adapt<PaymentMessage>(),
            BasketItemMessages : request.BasketItems.Adapt<List<BasketItemMessage>>()
        );

        await publishEndpoint.Publish(basketConfirmedEvent);
        return Results.Ok();
    }
}
