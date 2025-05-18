using Microsoft.AspNetCore.Authorization;

namespace Package.Identity.Authorization;

public class AuthorizationRequirement : IAuthorizationRequirement
{
    public string Permission { get; set; } = null!;
}