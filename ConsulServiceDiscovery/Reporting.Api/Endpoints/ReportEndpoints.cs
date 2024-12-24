using Contracts;

namespace Reporting.Api.Endpoints
{
    public static class ReportEndpoints
    {
        public static void MapReportEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/Report/{id}", (int id) =>
            {
                return new Response
                {
                    Id = id,//Random.Shared.Next(1, 50),
                    Message = $"From Report Endpoint for id:{id}",
                    CreatedOnUtc = DateTime.UtcNow,
                };
            });
        }
    }
}
