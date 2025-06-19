using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace YAEC.Shared.OpenApi.Swagger;

internal static class SwaggerExtensions
{
    /// <summary>
    /// Add swagger document
    /// </summary>
    /// <param name="services"></param>
    /// <param name="title">Set title for swagger document</param>
    public static void AddCoreSwagger(this IServiceCollection services, string title)
    {
        services.AddSwaggerGen(options =>
        {
            var apiVersionDescriptions = services.BuildServiceProvider()
                .GetRequiredService<IApiVersionDescriptionProvider>()
                .ApiVersionDescriptions;
            foreach (var apiVersionDescription in apiVersionDescriptions)
            {
                options.SwaggerDoc(apiVersionDescription.GroupName, new OpenApiInfo
                {
                    Title = title,
                    Version = apiVersionDescription.ApiVersion.ToString(),
                });
            }
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. " +
                    "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                    "\r\n\r\nExample: \"Bearer accessToken\"",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
    }
    
    /// <summary>
    /// Use Swagger UI
    /// </summary>
    /// <param name="app"></param>
    /// <param name="title">Set title for swagger page</param>
    public static void UseCoreSwagger(this WebApplication app, string title = "Swagger UI")
    {
        const string urlTemplate = "/swagger/{{DocumentName}}/swagger.json";
        const string routerPrefix = "swagger";
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = title;
            options.RoutePrefix = routerPrefix;
            var apiVersionDescriptions = app.DescribeApiVersions();
            foreach (var apiVersionDescription in apiVersionDescriptions)
            {
                var url = urlTemplate.Replace("{{DocumentName}}", apiVersionDescription.GroupName);
                var name = apiVersionDescription.GroupName;
                options.SwaggerEndpoint(url, name);
            }
        });
    }
}