using BasketApi.Services;

namespace BasketApi.Endpoints;

public static class BasketEndpoints
{
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Basket");

        group.MapGet("/{userName}", async (string userName, IBasketService basketService) =>
        {
            var shoppingCart = await basketService.GetBasket(userName);

            if (shoppingCart == null) return Results.NotFound();

            return Results.Ok(shoppingCart);

        })
        .WithName("GetBasket")
        .Produces<ShoppingCart>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        group.MapPost("/", async (ShoppingCart shoppingCart, IBasketService basketService) =>
        {
            await basketService.UpdateBasket(shoppingCart);
            return Results.Created("GetBasket", shoppingCart);
        })
        .WithName("UpdateBasket")
        .Produces<ShoppingCart>(StatusCodes.Status201Created)
        .RequireAuthorization();

        group.MapDelete("/{userName}", async(string userName, IBasketService basketService)=>
        {
            await basketService.DeleteBasket(userName);
            return Results.NoContent();
        })
        .WithName("DeleteBasket")
        .Produces(StatusCodes.Status204NoContent)
        .RequireAuthorization();
    }
}
