using AppreciateAppApi.Data;
using AppreciateAppApi.Models;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace AppreciateAppApi.Services;

public static class DatabaseSeedService
{
    public static async Task SeedDatabaseAsync(AppDbContext context)
    {
        if (context.Database.IsRelational())
        {
            bool isDataAdded = false;
            // Ensure the database is created
            await context.Database.EnsureCreatedAsync();
            
            if(await context.Employees.AnyAsync() == false)
            {
                // Seed initial employees
                var initialEmployees = GetInitialEmployees();
                await context.Employees.AddRangeAsync(initialEmployees);
                isDataAdded = true;
            }
            if(await context.Categories.AnyAsync() == false)
            {
                // Seed initial categories
                var initialCategories = CreateCategories();
                await context.Categories.AddRangeAsync(initialCategories);
                isDataAdded = true;
            }

            if (isDataAdded)
            {
                // Save changes to the database
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("No new data to seed. Database already contains initial data.");
                return;
            }

        }        
    }

    public static List<Employee> GetInitialEmployees()
    {
        return new Faker<Employee>()
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())            
            // username should be firstcharacter of lastname + first name
            .RuleFor(e=>e.UserName, (f, e) => $"{e.LastName.ToLower().Substring(0, 1)}{e.FirstName.ToLower()}")
            // Email should be firstcharacter of lastname + first name + "@example.com"
            .RuleFor(e=>e.Email, (f, e) => $"{e.LastName.ToLower().Substring(0, 1)}{e.FirstName.ToLower()}@example.com")
            .RuleFor(e => e.ProfilePictureUrl, f => f.Internet.Avatar())
            .RuleFor(e => e.CreatedAt, f => f.Date.Past(5).ToUniversalTime())
            .Generate(10);
    }

    public static List<Category> CreateCategories()
    {         return new List<Category>
        {
            new Category { Name = "Leadership", Description = "Leadership", ImageUrl="leadership.svg" },
            new Category { Name = "Awesome Code", Description = "Awesome Code", ImageUrl="awesome-code.svg" },
            new Category { Name = "Sharp Testing", Description = "Sharp Testing", ImageUrl="testing.svg" },
            new Category { Name = "Team Spirit", Description = "Team Spirit", ImageUrl="team-spirit.svg" },
            new Category { Name = "Customer Focus", Description = "Customer Focus", ImageUrl="customer-focus.svg" },
            new Category { Name = "Innovation", Description = "Innovation", ImageUrl="innovation.svg" },
            new Category { Name = "Thanks for your support", Description = "Thanks for your support", ImageUrl="support.svg" },
            new Category { Name = "Thanks for your help", Description = "Thanks for your help", ImageUrl="help.svg" },
            new Category { Name = "Thanks for your feedback", Description = "Thanks for your feedback", ImageUrl="feedback.svg" },
            new Category { Name = "Thanks for your great service", Description = "Thanks for your great service", ImageUrl="service.svg" },
            new Category { Name = "Great Job", Description = "Great Job", ImageUrl="great-job.svg" },
            new Category { Name = "Great Team", Description = "Great Team", ImageUrl="great-team.svg" }
        };
    }

}
