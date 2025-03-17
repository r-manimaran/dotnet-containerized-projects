using Aspire.eshop.CatalogDb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire.eshop.CatalogDb.Data;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    private static readonly Func<CatalogDbContext, int?, int?, int, IAsyncEnumerable<CatalogItem>> GetCatalogItemsAfterQuery =
        EF.CompileAsyncQuery((CatalogDbContext context, int? catalogBrandId, int? after, int pageSize) =>
            context.CatalogItems.AsNoTracking()
            .Where(ci => catalogBrandId == null || ci.CatalogBrandId == catalogBrandId)
            .Where(ci => after == null || ci.Id >= after)
            .OrderBy(ci => ci.Id)
            .Take(pageSize + 1));

    private static readonly Func<CatalogDbContext, int?, int, int, IAsyncEnumerable<CatalogItem>> GetCatalogItemsBeforeQuery =
        EF.CompileAsyncQuery((CatalogDbContext context, int? catalogBrandId, int before, int pageSize) =>
            context.CatalogItems.AsNoTracking()
            .Where(ci => catalogBrandId == null || ci.CatalogBrandId == catalogBrandId)
            .Where(ci => ci.Id <= before)
            .OrderByDescending(ci => ci.Id)
            .Take(pageSize + 1)
            .OrderBy(ci => ci.Id)
            .AsQueryable());


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();

    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();

    public Task<List<CatalogItem>> GetCatalogItemsCompiledAsync(int? catalogBrandId, int? before, int? after, int pageSize)
    {
        return ToListAsync(before is null
            // paging forward
            ? GetCatalogItemsAfterQuery(this, catalogBrandId, after, pageSize)
            // paging backward
            : GetCatalogItemsBeforeQuery(this, catalogBrandId, before.Value, pageSize));
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> asyncEnumerable)
    {
        var results = new List<T>();
        await foreach (var value in asyncEnumerable)
        {
            results.Add(value);
        }

        return results;
    }
}
