using Microsoft.CodeAnalysis.CSharp.Syntax;
using UserManagementApi.Features.Register;
using Wolverine;

namespace UserManagementApi.Endpoints;

public static class UserManagementEndpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("users", (RegisterUser command, IMessageBus bus) =>
        {
            bus.InvokeAsync(command);
        });
    }
}
