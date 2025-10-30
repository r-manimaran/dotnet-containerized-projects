
using Microsoft.AspNetCore.Authentication;

namespace Aspire.FrontEnd;

public class AuthorizationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContext available for the IHttpContextAccessor");
        var accessToken = await httpContext.GetTokenAsync("access_token");
        if(!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",accessToken);

        }
        return await base.SendAsync(request, cancellationToken);
    }
}
