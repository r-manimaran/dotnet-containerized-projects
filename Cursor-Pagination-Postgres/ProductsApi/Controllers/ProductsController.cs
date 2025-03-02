using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Pagination;

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

        [HttpGet("cursor")]
        public async Task<IActionResult> GetCursorPagination(string? cursor=null,
                                            int limit=10,
                                            CancellationToken cancellationToken=default)
        {
            if(limit < 1)
            {
                return BadRequest("limit should be greater than 0");
            }

            if(limit > 100)
            {
                return BadRequest("limit should be less than 100");
            }

            var query = dbContext.Products.AsQueryable();
            if (!string.IsNullOrEmpty(cursor))
            {
                var decodeCursor = Cursor.Decode(cursor);
                if(decodeCursor is null)
                {
                    return BadRequest("Invalid Cursor");
                }

                //query = query.Where(x => DateOnly.FromDateTime(x.CreatedDate) < decodeCursor.Date ||
                //                        DateOnly.FromDateTime(x.CreatedDate) == decodeCursor.Date && 
                //                            x.Id <= decodeCursor.LastId);
                // Tuple Query
                query = query.Where(x => EF.Functions.LessThanOrEqual(
                        ValueTuple.Create(DateOnly.FromDateTime(x.CreatedDate), x.Id),
                        ValueTuple.Create(decodeCursor.Date, decodeCursor.LastId)
                        ));
            }

            var items = await query
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.Id)
                .Take(limit + 1)
                .ToListAsync(cancellationToken);
            DateOnly? nextDate = items.Count > limit? DateOnly.FromDateTime(items[^1].CreatedDate) : null;
            Guid? nextId = items.Count>limit? items[^1].Id: null;
            items.RemoveAt(items.Count - 1);

            var response = new
            {
                Items = items,
                Cursor = nextDate is not null && nextId is not null ?
                        Cursor.Encode(nextDate.Value, nextId.Value) : null,
                HasMore = items.Count == limit
            };
            return Ok(response);

        }
    }
}
