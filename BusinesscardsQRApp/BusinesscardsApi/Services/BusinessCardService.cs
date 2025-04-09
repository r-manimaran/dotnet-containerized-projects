using BusinesscardsApi.Data;
using BusinesscardsApi.Models;
using BusinesscardsApi.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BusinesscardsApi.Services;

public interface IBusinessCardService
{
    Task<(IEnumerable<BusinessCard> businessCards, Metadata metadata)> GetBusinessCardUsers(RequestParameters parameters,CancellationToken cancellationToken);
}
public class BusinessCardService : IBusinessCardService
{
    private readonly AppDbContext _context;

    public BusinessCardService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<BusinessCard> businessCards, Metadata metadata)> GetBusinessCardUsers
                                        (RequestParameters parameters, CancellationToken cancellationToken)
    {
        var businessCards = await _context.BusinessCards.AsNoTracking()
                                    .Skip((parameters.PageNumber-1)* parameters.PageSize)
                                    .Take(parameters.PageSize)
                                    .ToListAsync();

        var count = await _context.BusinessCards.CountAsync(cancellationToken);

        var businessCardsWithMetadata  = PagedList<BusinessCard>.ToPagedList(businessCards, count, parameters.PageNumber, parameters.PageSize);

        return (businessCards: businessCards, metadata: businessCardsWithMetadata.Metadata);

    }
}
