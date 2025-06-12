using AppreciateAppApi.DTO;
using AppreciateAppApi.DTO.Appreciation;
using AppreciateAppApi.Models;
using AppreciateAppApi.Services;

namespace AppreciateAppApi.Endpoints;

public static class AppreciationEndpoints
{
    public static void MapAppreciationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Appreciation").WithTags("Appreciation").RequireAuthorization();

        group.MapGet("/send", async (int pageSize, int page, IAppreciationService raveService) =>
        {
            var raves = await raveService.GetAllAppreciation( page, pageSize, AppreciationType.Send);
            return Results.Ok(raves);
        }).Produces<BaseResponse<AppreciationResponse>>(StatusCodes.Status200OK);

        group.MapGet("/received", async (int pageSize, int page, IAppreciationService raveService) =>
        {
            var raves = await raveService.GetAllAppreciation(page, pageSize, AppreciationType.Received);
            return Results.Ok(raves);
        }).Produces<BaseResponse<AppreciationResponse>>(StatusCodes.Status200OK);

        group.MapPost("/", async (CreateAppreciationRequest appreciation, IAppreciationService raveService) =>
        {
            var newRave = await raveService.CreateAppreciationAsync(appreciation);
            return Results.Ok(newRave);
        });

        group.MapGet("/{id}", async (int id, IAppreciationService raveService) =>
        {
            var rave = await raveService.GetAppreciationByIdAsync(id);
            return rave != null ? Results.Ok(rave) : Results.NotFound();

        }).Produces<BaseResponse<Appreciation?>>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status404NotFound);
    }
}
