using Bogus;
using BusinesscardsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinesscardsApi.Data;

public static class DatabaseSeedService
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (await dbContext.BusinessCards.AnyAsync())
            return;

        var businesscardUsers = GenerateBusinessCards(100);

        await dbContext.BusinessCards.AddRangeAsync(businesscardUsers);
        await dbContext.SaveChangesAsync();
    }

    public static List<BusinessCard> GenerateBusinessCards(int count)
    {
        return new Faker<BusinessCard>()
            .RuleFor(b => b.FullName, f => f.Person.FullName)
            .RuleFor(b => b.Email, f => f.Person.Email)
            .RuleFor(b => b.Phone, f=> f.Phone.PhoneNumber())
            .RuleFor(b=>b.Title, f=>f.Person.UserName)
            .RuleFor(b=>b.CompanyName, f=>f.Company.CompanyName())
            .RuleFor(b=>b.Address, f=>f.Address.FullAddress())
            .RuleFor(b=>b.ProfileImageUrl, f=>f.Person.Avatar)
            .RuleFor(b=>b.Website, f=>f.Person.Website)
            .Generate(count);
    }
}
