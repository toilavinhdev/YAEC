using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Package.ObjectStorage;

public static class ObjectStorageExtensions
{
    public static void AddObjectStorage(this IServiceCollection services)
    {
        services.AddSingleton<IObjectStorageService, ObjectStorageService>();
    }

    public static async Task InitializeBucketAsync(WebApplication app)
    {
        var s3Manager = app.Services.GetRequiredService<IObjectStorageService>();
        await s3Manager.MakeBucketAsync();
    }
}