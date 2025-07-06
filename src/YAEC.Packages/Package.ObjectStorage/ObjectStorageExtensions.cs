using System.Text.Json;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Package.ObjectStorage;

public static class ObjectStorageExtensions
{
    public static void AddObjectStorage(this IServiceCollection services)
    {
        services.AddSingleton<IObjectStorageService, ObjectStorageService>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ObjectStorageService>>();
            var options = serviceProvider.GetRequiredService<ObjectStorageOptions>();

            var webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            var credentialPath = Path.Combine(webHostEnvironment.ContentRootPath, options.Credential);
            if (!File.Exists(credentialPath)) throw new FileNotFoundException($"File {credentialPath} not found.");
            var credentialsJson = File.ReadAllText(credentialPath);
            var credentials = JsonSerializer.Deserialize<Dictionary<string, string>>(credentialsJson) ?? new Dictionary<string, string>();
            var accessKey = credentials.GetValueOrDefault("accessKey");
            var secretKey = credentials.GetValueOrDefault("secretKey");

            var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            var amazonS3Config = new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl,
                ForcePathStyle = true,
                UseHttp = !options.Ssl,
            };
            var amazonS3Client = new AmazonS3Client(awsCredentials, amazonS3Config);
            return new ObjectStorageService(logger, amazonS3Client, options);
        });
    }
}