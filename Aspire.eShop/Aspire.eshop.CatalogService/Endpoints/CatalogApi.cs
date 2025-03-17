using Aspire.eshop.CatalogDb.Data;
using Aspire.eshop.CatalogDb.Models;

namespace Aspire.eshop.CatalogService.Endpoints;

public static class CatalogApi
{
    public static RouteGroupBuilder MapCatalogApiEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/catalog");
        
        group.WithTags("Catalog");

        group.MapGet("items/type/all", (CatalogDbContext dbContext, int? before, int? after, int pageSize = 8)
            => GetCatalogItems(null, dbContext, before, after, pageSize))
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<CatalogItemsPage>();

        group.MapGet("items/type/all/brand/{catalogBrandId:int}", (int catalogBrandId, CatalogDbContext catalogDbContext, int? before, int? after, int pageSize = 8)
            => GetCatalogItems(catalogBrandId, catalogDbContext, before, after, pageSize))
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<CatalogItemsPage>();

        group.MapGet("items/{catalogItemId:int}/image", async (int catalogItemId, CatalogDbContext catalogDbContext, IHostEnvironment environment)=>
        {
            var item = await catalogDbContext.CatalogItems.FindAsync(catalogItemId);

            if (item is null)
            {
                return Results.NotFound();
            }

            var path = Path.Combine(environment.ContentRootPath, "Images", item.PictureFileName);

            if (!File.Exists(path))
            {
                return Results.NotFound();
            }

            return Results.File(path, "image/jpeg");
        })
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(200, contentType:"image/jpeg");


        return group;
            
    }

    private static async Task<IResult> GetCatalogItems(int? catalogBrandId, CatalogDbContext dbContext, int? before, int? after, int pageSize)
    {
        if(before is > 0 && after is > 0)
        {
            return TypedResults.BadRequest($"Invalid paging Parameters. Only one of {nameof(before)} or {nameof(after)} can be specified, not both.");
        }
        // get results using the compiled query
        var itemsOnPage = await dbContext.GetCatalogItemsCompiledAsync(catalogBrandId, before, after, pageSize);

        var (firstId, nextId) = itemsOnPage switch
        {
            [] => (0, 0),
            [var only] => (only.Id, only.Id),
            [var first, .., var last] => (first.Id, last.Id)
        };

        return Results.Ok(new CatalogItemsPage(
            firstId,
            nextId,
            itemsOnPage.Count < pageSize,
            itemsOnPage.Take(pageSize)));

    }
}

public record CatalogItemsPage(int FirstId, int NextId, bool IsLastPage, IEnumerable<CatalogItem> Data);
