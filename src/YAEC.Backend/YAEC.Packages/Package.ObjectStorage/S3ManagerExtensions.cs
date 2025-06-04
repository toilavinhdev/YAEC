using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Package.ObjectStorage;

public static class S3ManagerExtensions
{
    public static void AddS3Manager(this IServiceCollection services)
    {
        services.AddSingleton<IS3Manager, S3Manager>();
    }

    public static async Task InitializeS3BucketAsync(this WebApplication app)
    {
        var s3Manager = app.Services.GetRequiredService<IS3Manager>();
        await s3Manager.MakeBucketAsync();
    }
}