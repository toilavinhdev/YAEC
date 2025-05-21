using Microsoft.AspNetCore.Mvc;
using Package.OpenApi.MinimalApi;
using Package.RabbitMQ.Services;
using Package.Shared.Models.Events;

namespace Service.Identity.Endpoints;

public class TestEndpoints : IEndpoints
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/test")
            .WithTags("Test");
        
        group.MapPost("/rabbitmq/publish", async
            ([FromBody] TestMessageBrokerEvent message, [FromServices] IRabbitMQProducerService<TestMessageBrokerEvent> messageBusClient) =>
            {
                await messageBusClient.PublishAsync(message);
            })
            .MapToApiVersion(1);
    }
}