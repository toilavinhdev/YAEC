using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Package.Identity.Extensions;

public static class HttpContextAccessorExtensions
{
    public static IdentityUserClaims UserClaims(this IHttpContextAccessor httpContextAccessor)
    {
        var accessToken = httpContextAccessor.HttpContext?.Request.Headers
            .FirstOrDefault(x => x.Key.Equals("Authorization"))
            .Value
            .ToString()
            .Split(" ")
            .LastOrDefault();
        if (string.IsNullOrEmpty(accessToken)) throw new UnauthorizedAccessException("Unauthorized");
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(accessToken);

        var id = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value ??
                 throw new NullReferenceException("Claim type 'id' cannot access");
        var email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ??
                    throw new NullReferenceException("Claim type 'email' cannot access");
        var phoneNumber = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "phoneNumber")?.Value ??
                          throw new NullReferenceException("Claim type 'phoneNumber' cannot access");
        var policies = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "policies")?.Value ??
                       throw new NullReferenceException("Claim type 'policies' cannot access");
        return new IdentityUserClaims
        {
            Id = id,
            Email = email,
            PhoneNumber = phoneNumber,
            Policies = policies.Split(",").ToList(),
        };
    }
}