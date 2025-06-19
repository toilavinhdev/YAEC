using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YAEC.Shared.Extensions;

public interface IAppSettings;

public static class EnvironmentExtensions
{
    public static WebApplicationBuilder WithEnvironment<TAppSettings>(this WebApplicationBuilder builder,
        string folderName = "AppSettings") where TAppSettings : class, IAppSettings, new()
    {
        var environment = builder.Environment;
        var environmentPath = Path.Combine(folderName, $"appsettings.{environment.EnvironmentName}.json");
        var appSettings = new TAppSettings();
        new ConfigurationBuilder()
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile(environmentPath)
            .AddEnvironmentVariables()
            .Build()
            .Bind(appSettings);
        builder.Services.AddAppSettings(appSettings);
        return builder;
    }

    private static void AddAppSettings<TAppSettings>(this IServiceCollection services, TAppSettings appSettings)
        where TAppSettings : class, IAppSettings, new()
    {
        services.AddSingleton(appSettings);
        var properties = typeof(TAppSettings).GetProperties();
        foreach (var property in properties)
        {
            if (!property.PropertyType.IsClass || property.PropertyType.IsPrimitive) continue;
            var propertyType = property.PropertyType;
            var propertyValue = property.GetValue(appSettings);
            if (propertyValue is null) throw new NullReferenceException($"Configuration '{propertyType.Name}' was not found");
            services.AddSingleton(propertyType, propertyValue);
        }
    }
}