using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using YAEC.Shared.OpenApi.Abstractions;

namespace Service.Catalog.Endpoints;

public class TestEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/test")
            .WithTags("Test");
        V1(group);
    }

    public void V1(RouteGroupBuilder group)
    {
        group.MapPost("/elasticsearch/index",
            ([FromServices] ElasticsearchClient elasticsearchClient) =>
            {
            })
            .MapToApiVersion(1);
    }
}