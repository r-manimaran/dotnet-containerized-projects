using AppreciateAppApi.DTO.Auth;

namespace AppreciateAppApi.Services;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    public Task<LoginResult> LoginAsync(LoginRequest request)
    {
        // Implementation here
        throw new NotImplementedException();
    }
}
