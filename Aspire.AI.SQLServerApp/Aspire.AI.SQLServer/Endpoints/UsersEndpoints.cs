using Aspire.AI.SQLServer.Data;
using Aspire.AI.SQLServer.Models;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Aspire.AI.SQLServer.Endpoints;

public static class UsersEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var app = routes.MapGroup("/users").WithTags("Users");

        app.MapPost("/Generate", async (AppDbContext context) =>
        {
            var faker = new Faker<User>();
            List<string> Interests = ["Reading", "Biking", "Bloging", "ContentCreation", "Exploring", "Learning"];
            
            
            var fakerUserProfile = new Faker<UserProfile>();
            fakerUserProfile.RuleFor(x => x.FirstName, f => f.Name.FirstName());
            fakerUserProfile.RuleFor(x => x.LastName, f => f.Name.LastName());
            fakerUserProfile.RuleFor(x => x.Email, f => f.Internet.Email());
            fakerUserProfile.RuleFor(x => x.Age, f => f.Random.Int(18, 65));
            fakerUserProfile.RuleFor(x => x.Interests, (f, u) => f.PickRandom(Interests, f.Random.Int(1, 3)).ToList());

            
            var fakerDetails = new Faker<UserProfileDetails>();          
            var fakerAddress = new Faker<Address>();            
            fakerAddress.RuleFor(x => x.City, f => f.Address.City());
            fakerAddress.RuleFor(x => x.State, f => f.Address.State());
            fakerAddress.RuleFor(x => x.Street, f => f.Address.StreetAddress());
            fakerAddress.RuleFor(x => x.ZipCode, f => f.Address.ZipCode());

            fakerDetails.RuleFor(x => x.Address, fakerAddress);
            fakerUserProfile.RuleFor(x => x.Details, fakerDetails);
           faker.RuleFor(x=>x.Profile, fakerUserProfile);

            var users = faker.Generate(1);
            var str = JsonSerializer.Serialize(users);
            foreach (var user in users)
            {
                context.Add(user);
            }           
            await context.SaveChangesAsync();
        });

        app.MapPost("/", async (AppDbContext context) =>
        {
            var user = new User()
            {
                Profile = new UserProfile()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Age = 30,
                    Interests = ["Blogging", "Coding", "Reading"],
                    Details = new UserProfileDetails
                    {
                        Address = new Address()
                        {
                            City = "Test",
                            State = "State",
                            Street = "Main Street",
                            ZipCode = "11001"
                        }
                    }
                }
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Results.Ok(user);
        });


        app.MapGet("/", async (AppDbContext context) =>
        {
            //var users = await context.Users.FirstOrDefaultAsync(x => x.Profile.Age == 30);  
            
            var age = await context.Users.Select(x => x.Profile.Age).FirstOrDefaultAsync();
            return Results.Ok(age);
        }).WithTags("Select");

        app.MapGet("/sum", async (AppDbContext context) =>
        {
            var ageSum =  await context.Users.SumAsync(x => x.Profile.Age);
            return Results.Ok(ageSum);
        }).WithTags("Select");

        app.MapGet("/AggregateAge", async(AppDbContext context) =>
        {
            var ageSum = await context.Users.AverageAsync(x => x.Profile.Age);
            return Results.Ok(ageSum);
        }).WithTags("Select");

        app.MapPut("/updateAge", async (AppDbContext context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync();
            user.Profile.Age = 40;
            await context.SaveChangesAsync();
            return Results.Ok(user);
        }).WithTags("Update");

        app.MapGet("/GetAllFirstName", async (AppDbContext context) =>
        {
            // var names = await context.Users.SelectMany(x => x.Profile.Interests).ToListAsync();
            var names = await context.Users.Select(x => x.Profile.FirstName).ToListAsync();
            return Results.Ok(names);
        }).WithTags("Select");

        app.MapGet("/GetAllInterests", async (AppDbContext context) =>
        {
            var names = await context.Users.SelectMany(x => x.Profile.Interests).ToListAsync();
            return Results.Ok(names);
        }).WithTags("Select");

        app.MapGet("/GetAllInterestsDistinct", async (AppDbContext context) =>
        {
            var names = await context.Users.SelectMany(x => x.Profile.Interests).Distinct().ToListAsync();
            return Results.Ok(names);
        }).WithTags("Select");

        app.MapGet("/GetAllInterestsDistinctCount", async (AppDbContext context) =>
        {
            var names = await context.Users.SelectMany(x => x.Profile.Interests).Distinct().CountAsync();
            return Results.Ok(names);
        }).WithTags("Select");

        app.MapGet("/GetFirstAndLastNameWithHypenSeparator", async (AppDbContext context) =>
        {
            var names = await context.Users.Select(x => x.Profile.FirstName + "-" + x.Profile.LastName).ToListAsync();
            return Results.Ok(names);
        }).WithTags("Select");

        // Filter users by age
        app.MapGet("/by-age/{age}", async (int age, AppDbContext context) =>
        {
            var users = await context.Users.Where(x => x.Profile.Age == age).ToListAsync();
            return Results.Ok(users);
        }).WithTags("Select");

        //Filter users based on interest
        app.MapGet("/by-interest/{interest}", async (string interest, AppDbContext context) =>
        {
            var users = await context.Users.Where(x => x.Profile.Interests.Contains(interest)).ToListAsync();
            return Results.Ok(users);
        }).WithTags("Select");

        // Filter by age range
        app.MapGet("/age-range/{min}/{max}", async (int min, int max, AppDbContext context) =>
        {
            var users = await context.Users.Where(x => x.Profile.Age >= min && x.Profile.Age <= max).ToListAsync();
            return Results.Ok(users);
        }).WithTags("Select");

        // Search by City
        app.MapGet("/by-city/{city}", async (string city, AppDbContext context) =>
        {
            var users = await context.Users.Where(x => x.Profile.Details.Address.City == city).ToListAsync();
            return Results.Ok(users);
        }).WithTags("Select");

        // Add Intereset to user
        app.MapPut("/{id}/add-interest", async(int id, string interest, AppDbContext context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user?.Profile.Interests.Contains(interest) == false)
            {
                user.Profile.Interests.Add(interest);
                await context.SaveChangesAsync();
            }      
            return Results.Ok(user);

        }).WithTags("Update");

        // Update Address
        app.MapPut("/{id}/address", async (int id, Address address, AppDbContext context) =>
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return Results.NotFound();
            }
            user.Profile.Details.Address = address;
            await context.SaveChangesAsync();
            return Results.Ok(user);
        }).WithTags("Update");

        // Update PhoneNumber
        app.MapPut("/{id}/phone-number", async (int id, string phoneNumber, AppDbContext context) =>
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return Results.NotFound();
            }
            // validate the phone number is in the format 111-111-1111
            
            user.Profile.Details.Phone = phoneNumber;
            await context.SaveChangesAsync();
            return Results.Ok(user);
        }).WithTags("Update");

        // Get User by PhoneNumber
        app.MapGet("/{phonenumber}", async (string phoneNumber, AppDbContext context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Profile.Details.Phone == phoneNumber);

            return Results.Ok(user);
        }).WithTags("Select");

        // Analytics 
        app.MapGet("/interest-stats", async (AppDbContext context) =>
        {
            var stats = await context.Users.SelectMany(u=>u.Profile.Interests)
            .GroupBy(i=>i)
            .Select(g=> new { interest = g.Key,Count=g.Count() })
            .OrderByDescending(x=>x.Count)
            .ToListAsync();
            return Results.Ok(stats);
        }).WithTags("Analytics");

        // Age demographics by city
        app.MapGet("/demographics", async (AppDbContext context) =>
        {
            var demographics = await context.Users
            .GroupBy(u=>u.Profile.Details.Address.City)
            .Select(g=> new
            {
                City=g.Key,
                AvgAge =g.Average(u=>u.Profile.Age),
                Count = g.Count()
            })
            .ToListAsync();
            return Results.Ok(demographics);
        });

        // Users with Multiple Interests
        app.MapGet("/multiple-interests", async (AppDbContext context) =>
        {
            var users = await context.Users
            .Where(u=>u.Profile.Interests.Count > 1)
            .Select(u=>new { u.Id, u.Profile.FirstName,InterestCount=u.Profile.Interests.Count})
            .ToListAsync();
            return Results.Ok(users);
        }).WithTags("Analytics");


        // Email domain Analysis
        app.MapGet("/email-domains", async (AppDbContext context) =>
        {
            var domains = await context.Users
                        .Select(u => u.Profile.Email.Substring(u.Profile.Email.IndexOf("@") + 1))
                        .GroupBy(d => d)
                        .Select(g => new { Domain = g.Key, Count = g.Count() })
                        .ToListAsync();
            return Results.Ok(domains);
        }).WithTags("Analytics");

    }


}
