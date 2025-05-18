using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Package.Identity.Authorization;

public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private const string PolicyPrefix = "YAEC";

    private readonly DefaultAuthorizationPolicyProvider _defaultAuthorizationPolicyProvider;

    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _defaultAuthorizationPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }
    
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _defaultAuthorizationPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _defaultAuthorizationPolicyProvider.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var pieces = policyName.Split(".");
        var permission = pieces[1];
        
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase) || pieces.Length != 2)
            return await _defaultAuthorizationPolicyProvider.GetPolicyAsync(policyName);
        
        var builder = new AuthorizationPolicyBuilder();
        builder.AddRequirements(new AuthorizationRequirement
        {
            Permission = permission
        });
        return builder.Build();
    }
}