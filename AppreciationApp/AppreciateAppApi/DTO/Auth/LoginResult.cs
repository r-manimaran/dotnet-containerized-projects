namespace AppreciateAppApi.DTO.Auth;

public class LoginResult
{
    public bool IsSuccess { get; set; } = false;
    public string? ErrorMessage { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiration { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}
