using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using YAEC.Shared.OpenApi.Swagger;

namespace YAEC.Shared.OpenApi;

public static class OpenApiExtensions
{
    public static void AddCoreOpenApi(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();
        services.AddEndpointsApiExplorer();
        services.AddCoreMinimalApis(assembly);
        services.AddCoreSwagger(assembly.GetName().Name!);
    }

    public static void UseCoreOpenApi(this WebApplication app, ApiVersionSet? apiVersionSet = null)
    {
        apiVersionSet ??= app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .HasApiVersion(new ApiVersion(2))
            .ReportApiVersions()
            .Build();
        app.UseCoreMinimalApis(apiVersionSet);
        app.UseCoreSwagger();
    }
}