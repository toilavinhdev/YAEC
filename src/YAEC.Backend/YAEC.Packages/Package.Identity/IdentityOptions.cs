namespace Package.Identity;

public class IdentityOptions
{
    public TokenOptions TokenOptions { get; set; } = null!;
}

public class TokenOptions
{
    public string TokenSigningKey { get; set; } = null!;
    
    public double AccessTokenDurationInMinutes { get; set; }

    public int RefreshTokenDurationInDays { get; set; }
    
    public string? Issuer { get; set; }

    public string? Audience { get; set; }
}