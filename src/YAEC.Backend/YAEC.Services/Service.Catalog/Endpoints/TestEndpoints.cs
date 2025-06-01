using Microsoft.AspNetCore.Mvc;
using Package.OpenApi.MinimalApi;
using Package.RabbitMQ.Services;
using Package.Redis;
using Package.Shared.Events;
using Package.Telegram;

namespace Service.Catalog.Endpoints;

public class TestEndpoints : IEndpoints
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/test/{ITelegramBotService}")
            .WithTags("Test");

        group.MapGet("/redis/get", async
            ([FromQuery] string key, [FromServices] IRedisService redisService) =>
            {
                return await redisService.StringGetAsync(key);
            })
            .MapToApiVersion(1);
        
        group.MapPost("/redis/set", async
            ([FromQuery] string key, [FromBody] string value, [FromServices] IRedisService redisService) =>
            {
                await redisService.StringSetAsync(key, value);
            })
            .MapToApiVersion(1);
        
        group.MapPost("/rabbitmq/publish", async
            ([FromBody] TestMessageBrokerEvent message, [FromServices] IRabbitMQProducerService<TestMessageBrokerEvent> messageBusClient) =>
            {
                await messageBusClient.PublishAsync(message);
            })
            .MapToApiVersion(1);
        
        group.MapPost("/telegram/send-message", async
            ([FromBody] string message, [FromServices] ITelegramBotService bot) =>
            {
                await bot.SendMessage(message);
            })
            .MapToApiVersion(1);
    }
}