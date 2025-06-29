namespace Service.Identity.Application.Auth.Responses;

public class SignInResponse
{
    public string AccessToken { get; set; } = null!;
    
    public DateTimeOffset AccessTokenExpiration { get; set; }

    public string RefreshToken { get; set; } = null!;
    
    public DateTimeOffset RefreshTokenExpiration { get; set; }
}