using Microsoft.CodeAnalysis.CSharp.Syntax;
using UserManagementApi.Features.Register;
using Wolverine;

namespace UserManagementApi.Endpoints;

public static class UserManagementEndpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("users", async(RegisterUser command, IMessageBus bus) =>
        {
            var id = await bus.InvokeAsync<Guid>(command);
            return Results.Ok(new { id });
        });
    }
}
