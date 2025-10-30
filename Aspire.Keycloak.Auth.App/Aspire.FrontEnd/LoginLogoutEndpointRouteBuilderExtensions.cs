using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Aspire.FrontEnd;

internal static class LoginLogoutEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapLoginLogoutEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("authentication");

        group.MapGet("/login",OnLogin).AllowAnonymous();
        group.MapGet("/logout", OnLogout);
        return group;
    }
    static ChallengeHttpResult OnLogin() => TypedResults.Challenge(properties: new Microsoft.AspNetCore.Authentication.AuthenticationProperties
    {
        RedirectUri = "/"
    });

    static SignOutHttpResult OnLogout() =>
        TypedResults.SignOut(properties: new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            RedirectUri = "/"
        }, 
        [
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
        ]);

}
