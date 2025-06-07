using AppreciateAppApi.DTO;
using AppreciateAppApi.DTO.Appreciation;
using AppreciateAppApi.Models;
using AppreciateAppApi.Services;

namespace AppreciateAppApi.Endpoints;

public static class AppreciationEndpoints
{
    public static void MapAppreciationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Appreciation").WithTags("Appreciation");

        group.MapGet("/send", async (int pageSize, int page, IAppreciationService raveService) =>
        {
            var raves = await raveService.GetAllAppreciation(pageSize, page, AppreciationType.Send);
            return Results.Ok(raves);
        }).Produces<BaseResponse<AppreciationResponse>>(StatusCodes.Status200OK);

        group.MapGet("/received", async (int pageSize, int page, IAppreciationService raveService) =>
        {
            var raves = await raveService.GetAllAppreciation(pageSize, page, AppreciationType.Received);
            return Results.Ok(raves);
        }).Produces<BaseResponse<AppreciationResponse>>(StatusCodes.Status200OK);

        group.MapPost("/", async (CreateAppreciationRequest appreciation, IAppreciationService raveService) =>
        {
            var newRave = await raveService.CreateAppreciationAsync(appreciation);
            return Results.Ok(newRave);
        });
    }
}
