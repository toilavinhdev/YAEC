using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using YAEC.Shared.Extensions;

namespace Package.Elasticsearch;

public interface IElasticsearchService
{
    
}

public class ElasticsearchService(ElasticsearchClient elasticsearchClient) : IElasticsearchService
{
    public async Task<bool> CreateIndexAsync(string indexName, Action<TypeMappingDescriptor> configure)
    {
        var createIndexResponse = await elasticsearchClient.Indices.CreateAsync(indexName.ToUnderscoreCase());
        return createIndexResponse.IsSuccess();
    }
    
    public async Task<T> IndexAsync<T>(string indexName, T document)
    {
        var indexResponse = await elasticsearchClient.IndexAsync(
            document,
            i =>
            {
                i.Index(indexName.ToUnderscoreCase());
            });
        return indexResponse.IsValidResponse ? document : default!;
    }
}