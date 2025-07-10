using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Package.Elasticsearch;

public static class ElasticsearchExtensions
{
    public static void AddSingleNodeElasticsearch(this IServiceCollection services)
    {
        var options = services.BuildServiceProvider().GetRequiredService<ElasticsearchOptions>();
        var settings = new ElasticsearchClientSettings(new Uri(options.Uri));
        settings.Authentication(new BasicAuthentication(options.Username, options.Password));
            
        var client = new ElasticsearchClient(settings);
        services.AddSingleton(client);
        services.AddSingleton<IElasticsearchService, ElasticsearchService>();
    }
}