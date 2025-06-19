using Microsoft.Extensions.DependencyInjection;

namespace Package.MongoDb;

public static class MongoDbExtensions
{
    public static void AddMongoDb(this IServiceCollection services)
    {
        services.AddSingleton<IMongoDbService, MongoDbService>();
    }
}