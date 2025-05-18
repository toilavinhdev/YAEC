using Microsoft.AspNetCore.Authorization;

namespace Package.Identity.Authorization;

public class AuthorizationHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var policy = requirement.Permission;
        var userClaimsId = context.User.Claims.FirstOrDefault(x => x.Type.Equals("id"))?.Value;
        var userClaimsPolicies = context.User.Claims.FirstOrDefault(x => x.Type.Equals("policies"))?.Value.Split(",");
        if (userClaimsId is null || userClaimsPolicies is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var hasRequirePolicy = userClaimsPolicies.Any(x => x == "All" || x == policy);
        if (!hasRequirePolicy)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}