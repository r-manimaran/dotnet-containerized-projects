using Blogging.Api.Data;
using Blogging.Api.Models;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Blogging.Api.Extensions;

public static class ApplicationExtensions
{
    public static void AddDatabaseService(this IServiceCollection services)
    {
        services.AddDbContext<BlogDbContext>(options =>
        {
            options.UseInMemoryDatabase("Blog");

            string[] categories = { "dotnet", "python", "genai" };
            options.UseAsyncSeeding(async (context, _, ct) =>
            {
                var categoryFaker = new Faker<Category>()
                            .UseSeed(1)
                            // Pick Randome Categories from the String
                            .RuleFor(x => x.Name, f => f.PickRandom(categories));
                // Generate 5 Posts for each Category

                var categiesToSeed = categoryFaker.Generate(5);
                var containsCategory = context.Set<Category>().Any();

                if (!containsCategory)
                {
                    await context.Set<Category>().AddRangeAsync(categiesToSeed);
                    await context.SaveChangesAsync(ct);
                }
                // Generate 10 posts. Use Faker Ruleset to generate data
                var postFaker = new Faker<Post>()
                .UseSeed(1)
                .RuleFor(x => x.Id, f => f.Random.Guid())
                // Generate Lorem Ipsum engine sentence


                .RuleFor(x => x.Title, f => f.Lorem.Text())
                .RuleFor(x => x.Content, f => f.Lorem.Paragraph())
                .RuleFor(x => x.CreatedDate, f => f.Date.Past(1))
                // Insert Random Category.
                .RuleFor(x => x.Category, f => f.PickRandom(categiesToSeed))
                .RuleFor(x => x.CategoryId, (f, post) => post.Category?.Id);

                var postsToSeed = postFaker.Generate(10);

                var containsPosts = await context.Set<Post>().ContainsAsync(postsToSeed[0], cancellationToken: ct);
                if (!containsPosts)
                {
                    await context.Set<Post>().AddRangeAsync(postsToSeed);
                    await context.SaveChangesAsync(ct);
                }
            });
        });
    }

    public static async void ApplyMigrationAndSeedData(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<BlogDbContext>())
            {
                await dbContext.Database.EnsureCreatedAsync();

            }
        }
    }

}
