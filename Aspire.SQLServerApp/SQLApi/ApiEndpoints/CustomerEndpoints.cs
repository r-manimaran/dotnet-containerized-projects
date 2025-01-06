using Dapper;
using Microsoft.Data.SqlClient;
using SQLApi.Models;

namespace SQLApi.ApiEndpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app) 
    {
        app.MapGet("/all-users", async (SqlConnection db) =>
        {
            const string sql = """
                SELECT Id, FirstName, LastName, Dob
                FROM Customers
            """;
            return await db.QueryAsync<Customer>(sql);
        }).WithTags("Customers");
    }
}
