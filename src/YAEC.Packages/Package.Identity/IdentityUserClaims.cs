using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace Package.Identity;

public class IdentityUserClaims
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = null!;
    
    [JsonPropertyName("policies")]
    public List<string> Policies { get; set; } = null!;
    
    public string GenerateAccessToken(IdentityOptions options)
    {
        return GenerateAccessToken(
            options.TokenSigningKey,
            options.AccessTokenDurationInMinutes,
            options.Issuer,
            options.Audience);
    }
    
    public string GenerateAccessToken(
        string signingKey,
        int accessTokenDurationInMinutes,
        string? issuer,
        string? audience)
    {
        var claims = new List<Claim>
        {
            new("id", Id),
            new("email", Email),
            new("phoneNumber", PhoneNumber),
            new("policies",  string.Join(",", Policies))
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(accessTokenDurationInMinutes);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            IssuedAt = DateTime.UtcNow,
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = credential
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);
        return accessToken;
    }
}