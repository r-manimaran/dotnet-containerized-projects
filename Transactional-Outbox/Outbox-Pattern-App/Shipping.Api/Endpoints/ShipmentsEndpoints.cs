using Shipping.Api.Services;

namespace Shipping.Api.Endpoints
{
    public static class ShipmentsEndpoints
    {
        public static void MapShipmentEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/Orders/{id}", async (Guid id, IShippingService shippingService) => 
            {
                var response = await shippingService.GetByOrderIdAsync(id);
                return response;
            })
            .WithName("GetOrderStatus")
            .WithOpenApi();
            
        }
    }
}
