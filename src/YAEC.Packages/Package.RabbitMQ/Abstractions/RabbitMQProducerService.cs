using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Package.RabbitMQ.Abstractions;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public interface IRabbitMQProducerService<in TMessage>
{
    Task PublishAsync(TMessage message, CancellationToken cancellationToken = default);
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class RabbitMQProducerService<TMessage> : IRabbitMQProducerService<TMessage>
{
    protected readonly ILogger<RabbitMQProducerService<TMessage>> Logger;
    
    private readonly IConnectionFactory _connectionFactory;
    
    protected abstract string ExchangeName { get; }

    protected string RoutingKey => $"{ExchangeName}_{typeof(TMessage).Name}";
    
    protected RabbitMQProducerService(IServiceProvider serviceProvider)
    {
        Logger = serviceProvider.GetRequiredService<ILogger<RabbitMQProducerService<TMessage>>>();
        _connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();
    }

    public async Task PublishAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await PublishAsync(channel, message, cancellationToken);
        await channel.CloseAsync(cancellationToken: cancellationToken);
        await connection.CloseAsync(cancellationToken: cancellationToken);
    }
    
    protected abstract Task PublishAsync(IChannel channel, TMessage message, CancellationToken cancellationToken = default);
}