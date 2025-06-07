using AppreciateAppApi.Services;

namespace AppreciateAppApi.Endpoints;

public static class EmployeesEndpoints
{
    public static void MapEmployeesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees").WithTags("Employees");
        
        group.MapGet("/me", async (IEmployeeService employeeService) =>
        {
            var employee = await employeeService.GetCurrentEmployeeAsync();
            return Results.Ok(employee);
        });

        group.MapGet("/{query}", async (string query, IEmployeeService employeeService) =>
        {
            var employees = await employeeService.SearchEmployeesAsync(query);
            return Results.Ok(employees);
        });

        group.MapGet("/profile/picture/{email}", async (string email, IEmployeeService employeeService) =>
        {
            var picture = await employeeService.GetProfilePictureAsync(email);
            return Results.Ok(picture);
        });
    }
}
