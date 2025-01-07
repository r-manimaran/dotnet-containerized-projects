
namespace PaymentApi.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder routes)
    {
        var app = routes.MapGroup("/api").WithTags("Payments");

        app.MapGet("/{orderId}", GetOrderStatus);
    }

    private static async Task GetOrderStatus(HttpContext context)
    {
        throw new NotImplementedException();
    }
}
