using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(AppDbContext dbContext) : ControllerBase
    {
        [HttpGet("offset")]
        public async Task<IActionResult> GetProductsOffset(int page=1, int pageSize=10,
                                                    CancellationToken cancellationToken=default)
        {
            var query = dbContext.Products.OrderByDescending(x => x.CreatedDate)
                                          .OrderByDescending(x => x.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount /(double)pageSize);

            var items = await query.Skip((page-1) * pageSize)                
                                   .Take(pageSize)
                                   .ToListAsync(cancellationToken);
            var results = new
            {

              Items = items,
              Page=page,
              PageSize=pageSize,
              TotalPages=totalPages,
              TotalCount=totalCount,
              HasNextPage = page < totalPages,
              HasPreviousPage = page > 1

            };

            return Ok(results);
        }


    }
}
