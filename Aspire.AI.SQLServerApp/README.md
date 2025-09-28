# SQL Server 2025 JSON Datatype Experiment

This project demonstrates the new JSON datatype capabilities in SQL Server 2025 using Entity Framework Core with .NET Aspire.

## Overview

The project showcases how to store, query, and manipulate JSON data in SQL Server 2025 using Entity Framework's `ToJson()` configuration. It includes comprehensive examples of JSON operations, filtering, aggregations, and analytics.

## Features

- **JSON Column Storage**: User profiles stored as native JSON datatype in SQL Server 2025
- **Complex JSON Queries**: Filtering and searching within JSON properties
- **JSON Aggregations**: Sum, average, and count operations on JSON fields
- **JSON Analytics**: Interest statistics, demographics, and domain analysis
- **CRUD Operations**: Create, read, update operations on JSON data

## Data Model

```csharp
User
├── Id (int)
└── Profile (JSON)
    ├── FirstName
    ├── LastName  
    ├── Email
    ├── Age
    ├── Interests (array)
    └── Details
        ├── Address
        │   ├── Street
        │   ├── City
        │   ├── State
        │   └── ZipCode
        ├── Phone
        └── Bio
```

## API Endpoints

### Data Generation
- `POST /users/Generate` - Generate fake user data using Bogus
- `POST /users/` - Create a sample user

### Basic Queries
- `GET /users/` - Get first user's age
- `GET /users/sum` - Sum all user ages
- `GET /users/AggregateAge` - Average age of all users
- `GET /users/GetAllFirstName` - Get all first names
```csharp
    // Get all first names in the profile
    var names = await context.Users.Select(x => x.Profile.FirstName).ToListAsync();
```

```sql
    SELECT JSON_VALUE([u].[Profile], '$.FirstName' RETURNING nvarchar(max))
    FROM [Users] AS [u]
```
- `GET /users/GetAllInterests` - Get all interests (with duplicates)
```csharp

```
```sql
SELECT [i].[value]
FROM [Users] AS [u]
CROSS APPLY OPENJSON(JSON_QUERY([u].[Profile], '$.Interests')) WITH ([value] nvarchar(max) '$') AS [i]
```
- `GET /users/GetAllInterestsDistinct` - Get unique interests
```csharp
```
```sql
SELECT DISTINCT [i].[value]
FROM [Users] AS [u]
CROSS APPLY OPENJSON(JSON_QUERY([u].[Profile], '$.Interests')) WITH ([value] nvarchar(max) '$') AS [i]
```


- `GET /users/GetAllInterestsDistinctCount` - Count unique interests
```csharp
 var names = await context.Users.SelectMany(x => x.Profile.Interests).Distinct().CountAsync();
```

```sql
SELECT COUNT(*)
FROM (
    SELECT DISTINCT [i].[value]
    FROM [Users] AS [u]
    CROSS APPLY OPENJSON(JSON_QUERY([u].[Profile], '$.Interests')) WITH ([value] nvarchar(max) '$') AS [i]
) AS [s]
```
- `GET /users/GetFirstAndLastNameWithHypenSeparator` - Concatenated names

```csharp
var names = await context.Users.Select(x => x.Profile.FirstName + "-" + x.Profile.LastName).ToListAsync();
```
```sql
SELECT JSON_VALUE([u].[Profile], '$.FirstName' RETURNING nvarchar(max)) + N'-' + JSON_VALUE([u].[Profile], '$.LastName' RETURNING nvarchar(max))
FROM [Users] AS [u]
```

### Filtering & Search
- `GET /users/by-age/{age}` - Filter users by specific age

```csharp
var users = await context.Users.Where(x => x.Profile.Age == age).ToListAsync();
```
```sql
    SELECT [u].[Id], [u].[Profile]
    FROM [Users] AS [u]
    WHERE JSON_VALUE([u].[Profile], '$.Age' RETURNING int) = @age
```
- `GET /users/by-interest/{interest}` - Filter users by interest

```csharp
var users = await context.Users.Where(x => x.Profile.Interests.Contains(interest)).ToListAsync();
```

```sql
    SELECT [u].[Id], [u].[Profile]
    FROM [Users] AS [u]
    WHERE @interest IN (
        SELECT [i].[value]
        FROM OPENJSON(JSON_QUERY([u].[Profile], '$.Interests')) WITH ([value] nvarchar(max) '$') AS [i]
    )
```
- `GET /users/age-range/{min}/{max}` - Filter users by age range

```csharp
var users = await context.Users.Where(x => x.Profile.Age >= min && x.Profile.Age <= max).ToListAsync();
```
```sql
    SELECT [u].[Id], [u].[Profile]
    FROM [Users] AS [u]
    WHERE JSON_VALUE([u].[Profile], '$.Age' RETURNING int) >= @min AND JSON_VALUE([u].[Profile], '$.Age' RETURNING int) <= @max
```

- `GET /users/by-city/{city}` - Filter users by city
```csharp
var users = await context.Users.Where(x => x.Profile.Details.Address.City == city).ToListAsync();
```
```sql
    SELECT [u].[Id], [u].[Profile]
    FROM [Users] AS [u]
    WHERE JSON_VALUE([u].[Profile], '$.Details.Address.City' RETURNING nvarchar(max)) = @city
```
- `GET /users/{phonenumber}` - Find user by phone number

```csharp
    var user = await context.Users.FirstOrDefaultAsync(x => x.Profile.Details.Phone == phoneNumber);
```

```sql
    SELECT TOP(1) [u].[Id], [u].[Profile]
    FROM [Users] AS [u]
    WHERE JSON_VALUE([u].[Profile], '$.Details.Phone' RETURNING nvarchar(max)) = @phoneNumber
```

### Updates
- `PUT /users/updateAge` - Update first user's age to 40
```csharp
 var user = await context.Users.FirstOrDefaultAsync();
            user.Profile.Age = 40;
            await context.SaveChangesAsync();
```
```sql

```
- `PUT /users/{id}/add-interest` - Add interest to user

```csharp
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user?.Profile.Interests.Contains(interest) == false)
            {
                user.Profile.Interests.Add(interest);
                await context.SaveChangesAsync();
            }      
```
```sql

```

- `PUT /users/{id}/address` - Update user's address

```csharp
    var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return Results.NotFound();
            }
            user.Profile.Details.Address = address;
            await context.SaveChangesAsync();
```

```sql
SET IMPLICIT_TRANSACTIONS OFF;
SET NOCOUNT ON;
UPDATE [Users] SET [Profile] = JSON_MODIFY([Profile], 'strict $.Details.Address', JSON_QUERY(@p0))
OUTPUT 1
WHERE [Id] = @p1;
```

- `PUT /users/{id}/phone-number` - Update user's phone number
```csharp
     var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return Results.NotFound();
        }
                    
        user.Profile.Details.Phone = phoneNumber;
        await context.SaveChangesAsync();
```
```sql
SET IMPLICIT_TRANSACTIONS OFF;
SET NOCOUNT ON;
UPDATE [Users] SET [Profile] = JSON_MODIFY([Profile], 'strict $.Details.Phone', @p0)
OUTPUT 1
WHERE [Id] = @p1;
```

### Analytics
- `GET /users/interest-stats` - Interest popularity statistics
```csharp
 var stats = await context.Users.SelectMany(u=>u.Profile.Interests)
            .GroupBy(i=>i)
            .Select(g=> new { interest = g.Key,Count=g.Count() })
            .OrderByDescending(x=>x.Count)
            .ToListAsync();
```
```sql
    SELECT [i].[value] AS [interest], COUNT(*) AS [Count]
    FROM [Users] AS [u]
    CROSS APPLY OPENJSON(JSON_QUERY([u].[Profile], '$.Interests')) WITH ([value] nvarchar(max) '$') AS [i]
    GROUP BY [i].[value]
    ORDER BY COUNT(*) DESC
```
- `GET /users/demographics` - Age demographics by city
```csharp
var demographics = await context.Users
            .GroupBy(u=>u.Profile.Details.Address.City)
            .Select(g=> new
            {
                City=g.Key,
                AvgAge =g.Average(u=>u.Profile.Age),
                Count = g.Count()
            })
            .ToListAsync();
```
```sql
SELECT [u0].[Key] AS [City], (
    SELECT AVG(CAST(JSON_VALUE([u1].[c], '$.Age' RETURNING int) AS float))
    FROM (
        SELECT [u2].[Profile] AS [c], JSON_VALUE([u2].[Profile], '$.Details.Address.City' RETURNING nvarchar(max)) AS [Key]
        FROM [Users] AS [u2]
    ) AS [u1]
    WHERE [u0].[Key] = [u1].[Key]) AS [AvgAge], COUNT(*) AS [Count]
FROM (
    SELECT JSON_VALUE([u].[Profile], '$.Details.Address.City' RETURNING nvarchar(max)) AS [Key]
    FROM [Users] AS [u]
) AS [u0]
GROUP BY [u0].[Key]
```
- `GET /users/multiple-interests` - Users with multiple interests
```csharp
 var users = await context.Users
            .Where(u=>u.Profile.Interests.Count > 1)
            .Select(u=>new { u.Id, u.Profile.FirstName,InterestCount=u.Profile.Interests.Count})
            .ToListAsync();
```
```sql
SELECT [u].[Id], JSON_VALUE([u].[Profile], '$.FirstName' RETURNING nvarchar(max)) AS [FirstName], (
    SELECT COUNT(*)
    FROM OPENJSON(JSON_QUERY([u].[Profile], '$.Interests')) AS [i0]) AS [InterestCount]
FROM [Users] AS [u]
WHERE (
    SELECT COUNT(*)
    FROM OPENJSON(JSON_QUERY([u].[Profile], '$.Interests')) AS [i]) > 1
```
- `GET /users/email-domains` - Email domain analysis
```csharp
    var domains = await context.Users
                            .Select(u => u.Profile.Email.Substring(u.Profile.Email.IndexOf("@") + 1))
                            .GroupBy(d => d)
                            .Select(g => new { Domain = g.Key, Count = g.Count() })
                            .ToListAsync();
```
```sql
SELECT [u0].[Key] AS [Domain], COUNT(*) AS [Count]
FROM (
    SELECT SUBSTRING(JSON_VALUE([u].[Profile], '$.Email' RETURNING nvarchar(max)), (CAST(CHARINDEX(N'@', JSON_VALUE([u].[Profile], '$.Email' RETURNING nvarchar(max))) AS int) - 1) + 1 + 1, LEN(JSON_VALUE([u].[Profile], '$.Email' RETURNING nvarchar(max)))) AS [Key]
    FROM [Users] AS [u]
) AS [u0]
GROUP BY [u0].[Key]
```

## Getting Started

### Prerequisites
- .NET 9.0+ SDK
- SQL Server 2025 (or compatible version with JSON support), AzureSQL
- Visual Studio 2022 or VS Code

### Running the Project

1. Clone the repository
2. Update connection string in `appsettings.json`
3. Run the project:
   ```bash
   dotnet run --project Aspire.AI.SQLServerApp.AppHost
   ```
4. Access Swagger UI at `http://localhost:5239/swagger`

### Key Configuration

The JSON column is configured in `UserConfiguration.cs`:

```csharp
builder.OwnsOne(t => t.Profile, b =>
{
    b.ToJson("Profile");
    b.OwnsOne(p => p.Details, d =>
    {
        d.OwnsOne(det => det.Address);
    });
});
```

## JSON Datatype Benefits

- **Native JSON Functions**: Leverage SQL Server's built-in JSON functions
- **Indexing**: Create indexes on JSON properties for better performance  
- **Type Safety**: Strongly-typed queries through Entity Framework
- **Flexibility**: Store semi-structured data without schema changes
- **Query Performance**: Optimized JSON queries compared to string operations

## Technology Stack

- **.NET Aspire**: Cloud-native application framework
- **Entity Framework Core**: ORM with JSON support
- **SQL Server 2025**: Database with native JSON datatype
- **Bogus**: Fake data generation
- **Swagger/OpenAPI**: API documentation