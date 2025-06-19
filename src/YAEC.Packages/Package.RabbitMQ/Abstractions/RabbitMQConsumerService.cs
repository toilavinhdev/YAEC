using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Package.RabbitMQ.Abstractions;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class RabbitMQConsumerService<TMessage> : BackgroundService
{
    protected readonly ILogger<RabbitMQConsumerService<TMessage>> Logger;
    
    private readonly IConnectionFactory _connectionFactory;
    
    protected abstract string ExchangeName { get; }

    protected virtual string RoutingKey => $"{typeof(TMessage).Name}";
    
    protected virtual string QueueName => $"{ExchangeName}_{typeof(TMessage).Name}";
    
    public RabbitMQConsumerService(IServiceProvider serviceProvider)
    {
        Logger = serviceProvider.GetRequiredService<ILogger<RabbitMQConsumerService<TMessage>>>();
        _connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await SubscribeAsync(channel, stoppingToken);
    }

    protected abstract Task SubscribeAsync(IChannel channel, CancellationToken cancellationToken);
}