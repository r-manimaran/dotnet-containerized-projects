using AppreciateAppApi.DTO.Auth;
using AppreciateAppApi.Services;

namespace AppreciateAppApi.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication");

        group.MapPost("/login", async (IAuthService authService, LoginRequest request) =>
        {
            var result = await authService.LoginAsync(request);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.Unauthorized();
        });
    }
}
