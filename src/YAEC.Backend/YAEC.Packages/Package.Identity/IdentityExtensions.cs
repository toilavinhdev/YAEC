using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Package.Identity.Authorization;

namespace Package.Identity;

public static class IdentityExtensions
{
    public static void AddCoreIdentity(this IServiceCollection services)
    {
        var options = services.BuildServiceProvider().GetRequiredService<IdentityOptions>();
        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultHandler>();
        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                var tokenOptions = options.TokenOptions;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.TokenSigningKey)),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = tokenOptions.Audience,
                    ValidateAudience = tokenOptions.Audience is not null,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidateIssuer = tokenOptions.Issuer is not null
                };
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var requestPath = context.HttpContext.Request.Path;
                        if (!requestPath.StartsWithSegments("/ws") ||
                            !context.Request.Query.TryGetValue("access_token", out var token))
                            return Task.CompletedTask;
                        context.Token = token;
                        context.Request.Headers.Authorization = token;
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();
    }

    public static void UseCoreIdentity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}