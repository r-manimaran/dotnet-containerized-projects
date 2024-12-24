using Carter;

namespace Orders.Api.Endpoints
{
    public class OrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/orders/{id}/report", async (int id, Client client) =>
            {
                var report = await client.GetAsync(id);
                if (report is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(report);
            });
        }
    }
}
