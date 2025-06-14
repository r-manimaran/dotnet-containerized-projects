using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;
using AppreciateAppApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace AppreciateAppApi.Endpoints;

public static class EmployeesEndpoints
{
    public static void MapEmployeesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees").WithTags("Employees").RequireAuthorization();
        
        group.MapGet("/me", async (IEmployeeService employeeService) =>
        {
            var employee = await employeeService.GetCurrentEmployeeAsync();
            return Results.Ok(employee);
        });

        group.MapGet("/", async (string query, IEmployeeService employeeService) =>
        {
            var employees = await employeeService.SearchEmployeesAsync(query);
            return Results.Ok(employees);
        }).Produces<BaseResponse<List<Employee>>>();

        group.MapGet("/profile/picture/{email}", async (string email, IEmployeeService employeeService) =>
        {
            var (imageBytes, contentType) = await employeeService.GetProfilePictureAsync(email);
            return imageBytes is not null
                    ? Results.File(imageBytes, contentType)
                    : Results.NotFound();
        });

        group.MapPost("/",async([FromForm]CreateEmployeeRequest request, IEmployeeService employeeService) =>
        {
            var newEmployee = await employeeService.CreateEmployeeAsync(request);
            return Results.Created($"/api/employees/{newEmployee.Id}", newEmployee);
        }).Produces<Employee>(StatusCodes.Status201Created);
    }
}
